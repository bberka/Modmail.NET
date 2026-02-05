using MediatR;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Features.Permission.Queries;

public sealed record GetPermissionLevelQuery(ulong UserId, bool IncludeRole = false) : IRequest<TeamPermissionLevel?>;