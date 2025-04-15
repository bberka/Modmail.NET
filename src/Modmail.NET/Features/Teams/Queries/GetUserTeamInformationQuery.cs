using Modmail.NET.Features.Teams.Models;

namespace Modmail.NET.Features.Teams.Queries;

public sealed record GetUserTeamInformationQuery : IRequest<UserTeamInformation[]>;