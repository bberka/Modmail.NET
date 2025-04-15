namespace Modmail.NET.Features.Teams.Queries;

public sealed record CheckTeamExistsQuery(ulong AuthorizedUserId, string Name) : IRequest<bool>;