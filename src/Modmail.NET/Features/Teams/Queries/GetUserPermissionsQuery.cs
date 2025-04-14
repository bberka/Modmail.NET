using MediatR;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Queries;

[CachePolicy(nameof(GetUserPermissionsQuery), 2)]
public sealed record GetUserPermissionsQuery(ulong UserId) : IRequest<AuthPolicy[]>;