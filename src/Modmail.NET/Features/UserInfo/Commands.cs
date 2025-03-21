using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Entities;

namespace Modmail.NET.Features.UserInfo;

public sealed record UpdateDiscordUserCommand(DiscordUser DiscordUser) : IRequest<DiscordUserInfo>;