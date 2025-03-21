using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessAddNoteHandler : IRequestHandler<ProcessAddNoteCommand>
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
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);
    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
    var noteEntity = new TicketNote {
      TicketId = ticket.Id,
      Content = request.Note,
      DiscordUserId = request.UserId
    };
    _dbContext.TicketNotes.Add(noteEntity);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _ = Task.Run(async () => {
      var user = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);

      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.NoteAdded(ticket, noteEntity, user));
      }

      var mailChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (mailChannel is not null) await mailChannel.SendMessageAsync(TicketResponses.NoteAdded(noteEntity, user));
    }, cancellationToken);
  }
}