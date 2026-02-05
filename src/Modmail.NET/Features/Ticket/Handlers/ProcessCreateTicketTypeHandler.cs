using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Language;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessCreateTicketTypeHandler : IRequestHandler<ProcessCreateTicketTypeCommand>
{
    private readonly ModmailDbContext _dbContext;
    private readonly ISender _sender;

    public ProcessCreateTicketTypeHandler(
        ModmailBot bot,
        ModmailDbContext dbContext,
        ISender sender
    )
    {
        _dbContext = dbContext;
        _sender = sender;
    }

    public async ValueTask<Unit> Handle(ProcessCreateTicketTypeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Name)) throw new ModmailBotException(Lang.InvalidName);


        var exists = await _sender.Send(new CheckTicketTypeExistsQuery(request.Name), cancellationToken);
        if (exists) throw new ModmailBotException(Lang.TicketTypeAlreadyExists);

        var id = Guid.CreateVersion7();
        var idClean = id.ToString()
            .Replace("-", "");
        if (request.Embed is not null) request.Embed.Id = Guid.CreateVersion7();

        var ticketType = new TicketType
        {
            Id = id,
            Key = idClean,
            Name = request.Name,
            Emoji = request.Emoji,
            Description = request.Description,
            Order = (int)request.Order,
            RegisterDateUtc = UtilDate.GetNow(),
            Embed = request.Embed,
            EmbedId = request.Embed?.Id
        };

        _dbContext.Add(ticketType);
        var affected = await _dbContext.SaveChangesAsync(cancellationToken);
        if (affected == 0) throw new DbInternalException();
        return Unit.Value;
    }
}