using MediatR;
using Modmail.NET.Abstract;

namespace Modmail.NET.Features.Teams.Queries;

public sealed record CheckTeamExistsQuery(ulong AuthorizedUserId, string Name) : IRequest<bool>,
                                                                                 IPermissionCheck;