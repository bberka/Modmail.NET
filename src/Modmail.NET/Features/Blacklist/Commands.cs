using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Blacklist;

public sealed record ProcessAddUserToBlacklistCommand(ulong UserId, string Reason = null, ulong ModId = 0) : IRequest<TicketBlacklist>;

public sealed record ProcessRemoveUserFromBlacklistCommand(ulong UserId, ulong AuthorUserId = 0) : IRequest<TicketBlacklist>;