using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;

namespace Modmail.NET.Features.TicketType.Handlers;

public class ProcessUpdateTicketTypeHandler : IRequestHandler<ProcessUpdateTicketTypeCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessUpdateTicketTypeHandler(ModmailDbContext dbContext,
                                        ISender sender,
                                        ModmailBot bot) {
    _dbContext = dbContext;
    _sender = sender;
    _bot = bot;
  }

  public async Task Handle(ProcessUpdateTicketTypeCommand request, CancellationToken cancellationToken) {
    var type = await _dbContext.TicketTypes.FindAsync([request.TicketType.Id], cancellationToken);
    if (type is null) throw new TicketTypeNotExistsException();

    var newType = request.TicketType;
    type.Name = newType.Name;
    type.Description = newType.Description;
    type.Emoji = newType.Emoji;
    type.Order = newType.Order;
    type.EmbedMessageTitle = newType.EmbedMessageTitle;
    type.EmbedMessageContent = newType.EmbedMessageContent;
    _dbContext.Update(type);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();
  }
}