using System.Text;
using DSharpPlus.Entities;
using Modmail.NET.Features.Ticket.Jobs;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketTypeChangedPrivateMessageHandler : INotificationHandler<NotifyTicketTypeChanged>
{
    private readonly ModmailBot _bot;
    private readonly TicketTypeSelectionTimeoutJob _ticketTypeSelectionTimeoutJob;

    public NotifyTicketTypeChangedPrivateMessageHandler(ModmailBot bot, TicketTypeSelectionTimeoutJob ticketTypeSelectionTimeoutJob)
    {
        _bot = bot;
        _ticketTypeSelectionTimeoutJob = ticketTypeSelectionTimeoutJob;
    }

    public async ValueTask Handle(NotifyTicketTypeChanged notification, CancellationToken cancellationToken)
    {
        if (notification.Ticket.BotTicketCreatedMessageInDmId == 0) return;

        var privateChannelId = notification.Ticket.PrivateMessageChannelId;
        var privateChannel = await _bot.Client.GetChannelAsync(privateChannelId);
        var privateMessageWithComponent = await privateChannel.GetMessageAsync(notification.Ticket.BotTicketCreatedMessageInDmId);
        var newEmbed = new DiscordEmbedBuilder(privateMessageWithComponent.Embeds[0]);
        if (notification.TicketType is not null)
        {
            var sb = new StringBuilder();
            if (DiscordEmoji.TryFromUnicode(_bot.Client, notification.TicketType.Emoji ?? string.Empty, out var emoji))
            {
                sb.Append(emoji);
                sb.Append(' ');
            }

            sb.Append(notification.TicketType.Name);
            newEmbed.AddField(Lang.TicketType.Translate(), sb.ToString());
        }

        await privateMessageWithComponent.ModifyAsync(x =>
        {
            x.ClearComponents();
            x.ClearEmbeds();
            x.AddEmbed(newEmbed);
        });

        _ticketTypeSelectionTimeoutJob.RemoveMessage(privateMessageWithComponent.Id);
    }
}