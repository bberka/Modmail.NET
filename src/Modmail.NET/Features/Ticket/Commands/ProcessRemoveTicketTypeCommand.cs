using Modmail.NET.Abstract;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessRemoveTicketTypeCommand(ulong AuthorizedUserId, Guid Id) : IRequest<TicketType>,
                                                                                       IPermissionCheck;