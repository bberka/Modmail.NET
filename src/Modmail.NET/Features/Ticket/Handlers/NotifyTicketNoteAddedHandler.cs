using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class NotifyTicketNoteAddedHandler : INotificationHandler<NotifyTicketNoteAdded>
{
    private readonly ModmailBot _bot;
    private readonly ISender _sender;

    public NotifyTicketNoteAddedHandler(ISender sender, ModmailBot bot)
    {
        _sender = sender;
        _bot = bot;
    }

    public async ValueTask Handle(NotifyTicketNoteAdded notification, CancellationToken cancellationToken)
    {
        var user = await _sender.Send(new GetDiscordUserInfoQuery(notification.AuthorizedUserId), cancellationToken);
        var mailChannel = await _bot.Client.GetChannelAsync(notification.Ticket.ModMessageChannelId);
        var embed = new DiscordEmbedBuilder().WithTitle(Lang.NoteAdded.Translate())
            .WithDescription(notification.Note.Content)
            .WithColor(ModmailColors.NoteAddedColor)
            .WithCustomTimestamp()
            .WithUserAsAuthor(user);
        await mailChannel.SendMessageAsync(embed);
    }
}