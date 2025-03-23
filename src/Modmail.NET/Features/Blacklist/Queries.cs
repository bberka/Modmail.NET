using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Blacklist;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record CheckUserBlacklistStatusQuery(ulong AuthorizedUserId,ulong DiscordUserId) : IRequest<bool>,
                                                                                                 IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetBlacklistQuery(
  ulong AuthorizedUserId,
  ulong DiscordUserId,
  bool AllowNull = false) : IRequest<TicketBlacklist>,
                            IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetAllBlacklistQuery(ulong AuthorizedUserId) : IRequest<List<TicketBlacklist>>,
                                                                    IPermissionCheck;