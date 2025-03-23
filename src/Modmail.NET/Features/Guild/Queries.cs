using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.Guild;

[CachePolicy("GetGuildOptionQuery", 60)]
public sealed record GetGuildOptionQuery(bool AllowNull) : IRequest<GuildOption>;

public sealed record CheckAnyGuildSetupQuery : IRequest<bool>;