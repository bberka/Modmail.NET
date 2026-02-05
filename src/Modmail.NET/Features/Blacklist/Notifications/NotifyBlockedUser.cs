namespace Modmail.NET.Features.Blacklist.Notifications;

public sealed record NotifyBlockedUser(
    ulong AuthorizedUserId,
    ulong UserId,
    string? Reason) : INotification;