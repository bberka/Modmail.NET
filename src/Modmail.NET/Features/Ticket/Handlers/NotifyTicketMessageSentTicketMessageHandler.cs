using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database;
using Modmail.NET.Features.Teams.Queries;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketMessageSentTicketMessageHandler : INotificationHandler<NotifyTicketMessageSent>
{
    private readonly ModmailBot _bot;
    private readonly ModmailDbContext _dbContext;
    private readonly ISender _sender;

    public NotifyTicketMessageSentTicketMessageHandler(
        ISender sender,
        ModmailDbContext dbContext,
        ModmailBot bot
    )
    {
        _sender = sender;
        _dbContext = dbContext;
        _bot = bot;
    }

    public async ValueTask Handle(NotifyTicketMessageSent notification, CancellationToken cancellationToken)
    {
        var message = await GetNewTicketMessage(notification, cancellationToken);
        var mailChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
        var botMessage = await mailChannel.SendMessageAsync(message);
        notification.Message.BotMessageId = botMessage.Id;
        _dbContext.Update(notification.Message);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();
    }

    private async ValueTask<DiscordMessageBuilder> GetNewTicketMessage(NotifyTicketMessageSent notification, CancellationToken cancellationToken)
    {
        var userInformation = await _sender.Send(new GetDiscordUserInfoQuery(notification.Message.SenderUserId), cancellationToken);
        var embed = new DiscordEmbedBuilder().WithCustomTimestamp()
            .WithUserAsAuthor(userInformation)
            .WithColor(ModmailColors.MessageReceivedColor);

        if (!string.IsNullOrEmpty(notification.Message.MessageContent)) embed.WithDescription(notification.Message.MessageContent);

        var msgBuilder = new DiscordMessageBuilder().AddEmbed(embed)
            .AddAttachments(notification.Message.Attachments.ToArray());

        var permissions = await _sender.Send(new GetUserTeamInformationQuery(), cancellationToken);
        msgBuilder.WithContent(UtilMention.GetNewMessagePingText(permissions));
        return msgBuilder;
    }
}