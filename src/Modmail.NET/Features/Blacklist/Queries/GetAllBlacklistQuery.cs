using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Blacklist.Queries;

[PermissionCheck(nameof(AuthPolicy.ManageBlacklist))]
public sealed record GetAllBlacklistQuery(ulong AuthorizedUserId) : IRequest<List<TicketBlacklist>>,
                                                                    IPermissionCheck;