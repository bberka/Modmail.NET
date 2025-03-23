using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Blacklist;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record ProcessAddUserToBlacklistCommand(ulong AuthorizedUserId, ulong UserId, string Reason = null) : IRequest<TicketBlacklist>,
                                                                                                                    IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record ProcessRemoveUserFromBlacklistCommand(ulong AuthorizedUserId, ulong UserId) : IRequest<TicketBlacklist>,
                                                                                                   IPermissionCheck;