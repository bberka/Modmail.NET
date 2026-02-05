using MediatR;

namespace Modmail.NET.Features.Tag.Commands;

//TODO: Permission check
public sealed record ProcessUpdateTagCommand(ulong AuthorizedUserId, Guid Id, string Name, string Title, string Content) : IRequest<Database.Entities.Tag>;