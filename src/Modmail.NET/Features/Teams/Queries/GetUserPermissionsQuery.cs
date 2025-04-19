using Modmail.NET.Attributes;

namespace Modmail.NET.Features.Teams.Queries;

[CachePolicy(nameof(GetUserPermissionsQuery), 2)]
public sealed record GetUserPermissionsQuery(ulong UserId) : IRequest<AuthPolicy[]>;