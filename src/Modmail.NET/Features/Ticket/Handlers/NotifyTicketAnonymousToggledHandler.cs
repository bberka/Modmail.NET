using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketAnonymousToggledHandler : INotificationHandler<NotifyTicketAnonymousToggled>
{
    private readonly ModmailBot _bot;

    public NotifyTicketAnonymousToggledHandler(ModmailBot bot)
    {
        _bot = bot;
    }

    public async ValueTask Handle(NotifyTicketAnonymousToggled notification, CancellationToken cancellationToken)
    {
        var ticketChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
        var title = notification.Ticket.Anonymous ? Lang.AnonymousModOn.Translate() : Lang.AnonymousModOff.Translate();

        var description = notification.Ticket.Anonymous
            ? Lang.TicketSetAnonymousDescription.Translate()
            : Lang.TicketSetNotAnonymousDescription.Translate();

        var user = await _bot.Client.GetUserAsync(notification.AuthorizedUserId);
        var embed = new DiscordEmbedBuilder().WithTitle(title)
            .WithColor(ModmailColors.AnonymousToggledColor)
            .WithCustomTimestamp()
            .WithUserAsAuthor(user)
            .WithDescription(description);
        await ticketChannel.SendMessageAsync(embed);
    }
}