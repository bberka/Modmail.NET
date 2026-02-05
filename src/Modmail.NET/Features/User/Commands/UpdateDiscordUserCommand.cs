using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.User.Commands;

public sealed record UpdateDiscordUserCommand(DiscordUser DiscordUser) : IRequest<DiscordUserInfo>;