using MediatR;

namespace Modmail.NET.Features.Permission.Queries;

public sealed record CheckUserInAnyTeamQuery(ulong MemberId) : IRequest<bool>;