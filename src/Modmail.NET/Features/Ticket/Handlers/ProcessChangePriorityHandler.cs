using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Notifications;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessChangePriorityHandler : IRequestHandler<ProcessChangePriorityCommand>
{
    private readonly ModmailDbContext _dbContext;
    private readonly IMediator _mediator;

    public ProcessChangePriorityHandler(IMediator mediator, ModmailDbContext dbContext)
    {
        _mediator = mediator;
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(ProcessChangePriorityCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _dbContext.Tickets.FilterActive()
            .FilterById(request.TicketId)
            .FirstOrDefaultAsync(cancellationToken);
        if (ticket is null) throw new ModmailBotException(Lang.TicketNotFound);

        var oldPriority = ticket.Priority;
        ticket.Priority = request.NewPriority;
        _dbContext.Update(ticket);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await _mediator.Publish(new NotifyTicketPriorityChanged(request.AuthorizedUserId, ticket, oldPriority, request.NewPriority),
            cancellationToken);
        return Unit.Value;
    }
}