using Modmail.NET.Attributes;
using Modmail.NET.Features.Teams.Models;

namespace Modmail.NET.Features.Teams.Queries;

[CachePolicy("GetUserTeamInformationQuery", 5)]
public sealed record GetUserTeamInformationQuery : IRequest<UserTeamInformation[]>;