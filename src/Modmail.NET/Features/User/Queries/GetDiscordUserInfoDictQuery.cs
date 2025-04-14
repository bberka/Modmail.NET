using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.User.Queries;

[CachePolicy("GetDiscordUserInfoQuery", 5)]
public sealed record GetDiscordUserInfoDictQuery : IRequest<Dictionary<ulong, UserInformation>>;