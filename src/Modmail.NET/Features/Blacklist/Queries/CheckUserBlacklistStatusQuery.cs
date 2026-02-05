using MediatR;

namespace Modmail.NET.Features.Blacklist.Queries;

public sealed record CheckUserBlacklistStatusQuery(ulong DiscordUserId) : IRequest<bool>;