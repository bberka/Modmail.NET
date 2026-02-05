using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Guild.Queries;

[CachePolicy("GetGuildOptionQuery", 60)]
public sealed record GetGuildOptionQuery(bool AllowNull) : IRequest<GuildOption>;