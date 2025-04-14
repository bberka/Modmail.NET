using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.User.Queries;

[CachePolicy("GetDiscordUserInfoQuery", 60)]
public sealed record GetDiscordUserInfoQuery(ulong UserId) : IRequest<UserInformation>;