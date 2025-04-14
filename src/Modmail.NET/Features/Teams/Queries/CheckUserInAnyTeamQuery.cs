using MediatR;

namespace Modmail.NET.Features.Teams.Queries;

public sealed record CheckUserInAnyTeamQuery(ulong UserId) : IRequest<bool>;