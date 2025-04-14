using DSharpPlus.Exceptions;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Database.Entities;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.User.Queries;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessAddNoteHandler : IRequestHandler<ProcessAddNoteCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessAddNoteHandler(ModmailBot bot,
                               ModmailDbContext dbContext,
                               ISender sender) {
    _bot = bot;
    _dbContext = dbContext;
    _sender = sender;
  }

  public async Task Handle(ProcessAddNoteCommand request, CancellationToken cancellationToken) {
    var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
    ticket.ThrowIfNotOpen();
    var noteEntity = new TicketNote {
      TicketId = ticket.Id,
      Content = request.Note,
      UserId = request.UserId
    };
    _dbContext.Add(noteEntity);
    await _dbContext.SaveChangesAsync(cancellationToken);
    _ = Task.Run(async () => {
      try {
        var user = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);
        var mailChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
        await mailChannel.SendMessageAsync(TicketBotMessages.Ticket.NoteAdded(noteEntity, user));
      }
      catch (NotFoundException) {
        //ignored
      }
    }, cancellationToken);
  }
}