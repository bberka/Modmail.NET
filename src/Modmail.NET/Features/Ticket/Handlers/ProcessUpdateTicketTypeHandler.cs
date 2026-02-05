using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessUpdateTicketTypeHandler : IRequestHandler<ProcessUpdateTicketTypeCommand>
{
    private readonly ModmailDbContext _dbContext;

    public ProcessUpdateTicketTypeHandler(ModmailDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<Unit> Handle(ProcessUpdateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        var type = await _dbContext.TicketTypes.FilterById(request.TicketType.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (type is null) throw new ModmailBotException(Lang.TicketTypeNotExists);

        _dbContext.Update(request.TicketType);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();
        return Unit.Value;
    }
}