using Modmail.NET.Attributes;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.User.Queries;

[CachePolicy("GetDiscordUserInfoQuery", 5)]
public sealed record GetDiscordUserInfoListQuery : IRequest<List<UserInformation>>;