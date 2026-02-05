using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Ticket.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTicketTypes))]
public sealed record ProcessCreateTicketTypeCommand(
    ulong AuthorizedUserId,
    string Name,
    string Emoji,
    string Description,
    long Order,
    Embed? Embed = null) : IRequest, IPermissionCheck;