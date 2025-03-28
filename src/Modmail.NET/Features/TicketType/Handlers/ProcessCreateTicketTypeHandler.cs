using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.TicketType.Handlers;

public class ProcessCreateTicketTypeHandler : IRequestHandler<ProcessCreateTicketTypeCommand>
{
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessCreateTicketTypeHandler(ModmailBot bot,
                                        ModmailDbContext dbContext,
                                        ISender sender) {
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessCreateTicketTypeCommand request, CancellationToken cancellationToken) {
    if (string.IsNullOrEmpty(request.Name)) throw new InvalidNameException(request.Name);


    var exists = await _sender.Send(new CheckTicketTypeExistsQuery(request.Name), cancellationToken);
    if (exists) throw new TicketTypeAlreadyExistsException(request.Name);

    var id = Guid.CreateVersion7();
    var idClean = id.ToString().Replace("-", "");
    var ticketType = new Entities.TicketType {
      Id = id,
      Key = idClean,
      Name = request.Name,
      Emoji = request.Emoji,
      Description = request.Description,
      Order = (int)request.Order,
      RegisterDateUtc = UtilDate.GetNow(),
      EmbedMessageTitle = request.EmbedMessageTitle,
      EmbedMessageContent = request.EmbedMessageContent
    };

    _dbContext.Add(ticketType);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}