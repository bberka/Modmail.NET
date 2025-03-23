using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessModSendMessageHandler : IRequestHandler<ProcessModSendMessageCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessModSendMessageHandler(ISender sender,
                                      ModmailBot bot,
                                      ModmailDbContext dbContext) {
    _sender = sender;
    _bot = bot;
    _dbContext = dbContext;
  }

  public async Task Handle(ProcessModSendMessageCommand request, CancellationToken cancellationToken) {
    ArgumentNullException.ThrowIfNull(request.ModUser);
    ArgumentNullException.ThrowIfNull(request.Message);
    ArgumentNullException.ThrowIfNull(request.Channel);
    ArgumentNullException.ThrowIfNull(request.Guild);

    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);
    ticket.LastMessageDateUtc = DateTime.UtcNow;

    _dbContext.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
    _ = Task.Run(async () => {
      var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      if (privateChannel is not null) {
        var embed = UserResponses.MessageReceived(request.Message, ticket.Anonymous);
        await privateChannel.SendMessageAsync(embed);
      }

      var ticketChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (ticketChannel is not null) {
        var embed2 = TicketResponses.MessageSent(request.Message, ticket.Anonymous);
        await ticketChannel.SendMessageAsync(embed2);
        await request.Message.DeleteAsync();
      }

      var ticketMessage = TicketMessage.MapFrom(request.TicketId, request.Message, true);
      await _dbContext.AddAsync(ticketMessage, cancellationToken);
      var affected2 = await _dbContext.SaveChangesAsync(cancellationToken);
      if (affected2 == 0) throw new DbInternalException();
    }, cancellationToken);
  }
}