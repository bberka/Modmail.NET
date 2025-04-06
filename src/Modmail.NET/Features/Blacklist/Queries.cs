using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Blacklist;

public sealed record CheckUserBlacklistStatusQuery(ulong DiscordUserId) : IRequest<bool>;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetBlacklistQuery(
  ulong AuthorizedUserId,
  ulong DiscordUserId,
  bool AllowNull = false) : IRequest<TicketBlacklist>,
                            IPermissionCheck;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetAllBlacklistQuery(ulong AuthorizedUserId) : IRequest<List<TicketBlacklist>>,
                                                                    IPermissionCheck;