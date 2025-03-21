using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Blacklist;

public sealed record CheckUserBlacklistStatusQuery(ulong DiscordUserId) : IRequest<bool>;

public sealed record GetBlacklistQuery(
  ulong DiscordUserId,
  bool AllowNull = false) : IRequest<TicketBlacklist>;

public sealed record GetAllBlacklistQuery : IRequest<List<TicketBlacklist>>;