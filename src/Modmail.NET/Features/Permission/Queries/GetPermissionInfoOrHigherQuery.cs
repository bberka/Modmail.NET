using MediatR;
using Modmail.NET.Features.Permission.Models;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Features.Permission.Queries;

public sealed record GetPermissionInfoOrHigherQuery(TeamPermissionLevel LevelOrHigher) : IRequest<PermissionInfo[]>;