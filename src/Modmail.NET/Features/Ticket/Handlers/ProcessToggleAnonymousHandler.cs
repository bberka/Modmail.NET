using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessToggleAnonymousHandler : IRequestHandler<ProcessToggleAnonymousCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessToggleAnonymousHandler(ISender sender,
                                       ModmailBot bot,
                                       ModmailDbContext dbContext) {
    _sender = sender;
    _bot = bot;
    _dbContext = dbContext;
  }


  public async Task Handle(ProcessToggleAnonymousCommand request, CancellationToken cancellationToken) {
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);


    ticket.Anonymous = !ticket.Anonymous;
    _dbContext.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketResponses.AnonymousToggled(ticket));

      //TODO: Handle mail channel not found
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.AnonymousToggled(ticket));
      }
    }, cancellationToken);
  }
}