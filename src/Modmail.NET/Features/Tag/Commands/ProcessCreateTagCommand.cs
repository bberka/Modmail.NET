using MediatR;

namespace Modmail.NET.Features.Tag.Commands;

//TODO: Permission check
public sealed record ProcessCreateTagCommand(ulong AuthorizedUserId, string Name, string Title, string Content) : IRequest<Database.Entities.Tag>;