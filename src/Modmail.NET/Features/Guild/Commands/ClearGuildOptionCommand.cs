using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;

namespace Modmail.NET.Features.Guild.Commands;

[PermissionCheck(nameof(AuthPolicy.Owner))]
public sealed record ClearGuildOptionCommand(ulong AuthorizedUserId) : IRequest,
                                                                       IPermissionCheck;