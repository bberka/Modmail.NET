using MediatR;
using Modmail.NET.Abstract;
using Modmail.NET.Attributes;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Commands;

[PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessRemoveTicketTypeCommand(ulong AuthorizedUserId, Guid Id) : IRequest<TicketType>,
                                                                                       IPermissionCheck;