using MediatR;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Queries;

public sealed record CheckPermissionAccessQuery(ulong UserId, AuthPolicy Policy) : IRequest<bool>;