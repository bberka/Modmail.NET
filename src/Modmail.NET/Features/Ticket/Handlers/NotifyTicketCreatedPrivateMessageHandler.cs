using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Jobs;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketCreatedPrivateMessageHandler : INotificationHandler<NotifyTicketCreated>
{
    private readonly ModmailBot _bot;
    private readonly ModmailDbContext _dbContext;
    private readonly ISender _sender;
    private readonly TicketTypeSelectionTimeoutJob _ticketTypeSelectionTimeoutJob;

    public NotifyTicketCreatedPrivateMessageHandler(
        ISender sender,
        ModmailDbContext dbContext,
        TicketTypeSelectionTimeoutJob ticketTypeSelectionTimeoutJob,
        ModmailBot bot
    )
    {
        _sender = sender;
        _dbContext = dbContext;
        _ticketTypeSelectionTimeoutJob = ticketTypeSelectionTimeoutJob;
        _bot = bot;
    }

    public async ValueTask Handle(NotifyTicketCreated notification, CancellationToken cancellationToken)
    {
        var channel = await _bot.Client.GetChannelAsync(notification.Ticket.PrivateMessageChannelId);
        var option = await _sender.Send(new GetOptionQuery(), cancellationToken);

        var ticketTypes = await _dbContext.TicketTypes.ToListAsync(cancellationToken);
        var ticketCreatedMessage = await channel.SendMessageAsync(YouHaveCreatedNewTicket(option, ticketTypes, notification.Ticket.Id));

        _ticketTypeSelectionTimeoutJob.AddMessage(ticketCreatedMessage);
        notification.Ticket.BotTicketCreatedMessageInDmId = ticketCreatedMessage.Id;
        _dbContext.Update(notification);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();
    }


    private static DiscordMessageBuilder YouHaveCreatedNewTicket(
        Option option,
        List<TicketType> ticketTypes,
        Guid ticketId
    )
    {
        var embed = new DiscordEmbedBuilder().WithTitle(Lang.YouHaveCreatedNewTicket.Translate())
            .WithFooter(option.Name, option.IconUrl)
            .WithCustomTimestamp()
            .WithColor(ModmailColors.TicketCreatedColor);
        var greetingMessage = Lang.GreetingMessageDescription.Translate();
        if (!string.IsNullOrEmpty(greetingMessage))
            embed.WithDescription(greetingMessage);

        var builder = new DiscordMessageBuilder().AddEmbed(embed);

        if (ticketTypes.Count > 0)
        {
            var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                Lang.PleaseSelectATicketType.Translate(), ticketTypes
                    .Select(x => string.IsNullOrWhiteSpace(x.Emoji)
                        ? new DiscordSelectComponentOption(x.Name, x.Key.ToString(), x.Description)
                        : new DiscordSelectComponentOption(x.Name, x.Key.ToString(), x.Description, false, new DiscordComponentEmoji(x.Emoji)))
                    .ToList());
            builder.AddComponents(selectBox);
        }

        return builder;
    }
}