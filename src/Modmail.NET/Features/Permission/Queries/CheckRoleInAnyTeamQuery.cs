using MediatR;

namespace Modmail.NET.Features.Permission.Queries;

public sealed record CheckRoleInAnyTeamQuery(ulong RoleId) : IRequest<bool>;