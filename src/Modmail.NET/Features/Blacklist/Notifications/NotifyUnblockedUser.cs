namespace Modmail.NET.Features.Blacklist.Notifications;

public sealed record NotifyUnblockedUser(ulong AuthorizedUserId, ulong UserId) : INotification;