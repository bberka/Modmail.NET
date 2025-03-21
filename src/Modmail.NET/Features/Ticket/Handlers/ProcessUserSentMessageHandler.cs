using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.Teams;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessUserSentMessageHandler : IRequestHandler<ProcessUserSentMessageCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessUserSentMessageHandler(ModmailDbContext dbContext,
                                       ModmailBot bot,
                                       ISender sender) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
  }

  public async Task Handle(ProcessUserSentMessageCommand request, CancellationToken cancellationToken) {
    ArgumentNullException.ThrowIfNull(request.Message);
    await Task.Delay(50, cancellationToken); //wait for privateChannel creation process to finish
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);

    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    ticket.LastMessageDateUtc = DateTime.UtcNow;

    _dbContext.Update(ticket);
    var ticketMessage = TicketMessage.MapFrom(ticket.Id, request.Message);

    _dbContext.Add(ticketMessage);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    _ = Task.Run(async () => {
      var privateChannel = request.PrivateChannel ?? await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      if (privateChannel is null) throw new NotFoundWithException(LangKeys.CHANNEL, ticket.PrivateMessageChannelId);

      var mailChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (mailChannel is not null) {
        var permissions = await _sender.Send(new GetTeamPermissionInfoQuery(), cancellationToken);
        await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(request.Message, permissions));
      }

      //TODO: Handle mail privateChannel not found
      if (guildOption.IsEnableDiscordChannelLogging)
        if (guildOption.IsSensitiveLogging) {
          await privateChannel.SendMessageAsync(UserResponses.MessageSent(request.Message));

          var logChannel = await _bot.GetLogChannelAsync();
          await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(request.Message, ticket.Id));
        }
    }, cancellationToken);
  }
}