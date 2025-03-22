using MediatR;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessArchiveTicketHandler : IRequestHandler<ProcessArchiveTicketCommand>
{
  public Task Handle(ProcessArchiveTicketCommand request, CancellationToken cancellationToken) {
    throw new NotImplementedException();
  }
}