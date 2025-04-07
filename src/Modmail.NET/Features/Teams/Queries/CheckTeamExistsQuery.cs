using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Teams.Queries;

[PermissionCheck(nameof(AuthPolicy.ManageTeams))]
public sealed record CheckTeamExistsQuery(ulong AuthorizedUserId, string Name) : IRequest<bool>,
                                                                                 IPermissionCheck;