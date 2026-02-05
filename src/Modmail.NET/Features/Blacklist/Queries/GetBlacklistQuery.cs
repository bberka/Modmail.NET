using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Blacklist.Queries;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetBlacklistQuery(
  ulong AuthorizedUserId,
  ulong DiscordUserId,
  bool AllowNull = false) : IRequest<TicketBlacklist>,
                            IPermissionCheck;