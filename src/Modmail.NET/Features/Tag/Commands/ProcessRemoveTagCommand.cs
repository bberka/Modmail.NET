using MediatR;

namespace Modmail.NET.Features.Tag.Commands;

//TODO: Permission check
// [PermissionCheck(nameof(AuthPolicy.ManageTicketTypes))] 
public sealed record ProcessRemoveTagCommand(ulong AuthorizedUserId, Guid Id) : IRequest<Database.Entities.Tag>;