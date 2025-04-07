using MediatR;
using Modmail.NET.Features.Permission.Models;

namespace Modmail.NET.Features.Permission.Queries;

public sealed record GetPermissionInfoQuery : IRequest<PermissionInfo[]>;