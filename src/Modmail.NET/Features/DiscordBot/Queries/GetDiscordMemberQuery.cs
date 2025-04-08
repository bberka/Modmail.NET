using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.DiscordBot.Queries;

public sealed record GetDiscordMemberQuery(ulong UserId) : IRequest<DiscordMember>;