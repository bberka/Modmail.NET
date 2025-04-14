using MediatR;
using Modmail.NET.Abstract;

namespace Modmail.NET.Features.Teams.Commands;

public sealed record SyncSuperUserTeamUsersCommand(ulong AuthorizedUserId) : IRequest,
                                                                             IPermissionCheck;