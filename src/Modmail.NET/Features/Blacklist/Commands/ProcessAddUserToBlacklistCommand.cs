using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Blacklist.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record ProcessAddUserToBlacklistCommand(ulong AuthorizedUserId, ulong UserId, string Reason = null) : IRequest<TicketBlacklist>,
                                                                                                                    IPermissionCheck;