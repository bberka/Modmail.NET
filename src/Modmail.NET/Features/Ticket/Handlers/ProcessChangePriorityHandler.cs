using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessChangePriorityHandler : IRequestHandler<ProcessChangePriorityCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessChangePriorityHandler(ISender sender,
                                      ModmailDbContext dbContext,
                                      ModmailBot bot) {
    _sender = sender;
    _dbContext = dbContext;
    _bot = bot;
  }

  public async Task Handle(ProcessChangePriorityCommand request, CancellationToken cancellationToken) {
    if (request.ModUserId == 0) throw new InvalidUserIdException();

    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId), cancellationToken);
    if (ticket.ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();

    var oldPriority = ticket.Priority;
    ticket.Priority = request.NewPriority;


    _dbContext.Tickets.Update(ticket);
    await _dbContext.SaveChangesAsync(cancellationToken);

    _ = Task.Run(async () => {
      //Don't await this task
      // var modUser = await ModmailBot.This.Client.GetUserAsync(modUserId);
      // if (modUser is null) throw new InvalidOperationException("ModUser is null");

      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(request.ModUserId), cancellationToken);
      if (modUser is null) throw new InvalidOperationException("ModUser is null");


      var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      if (privateChannel is not null) await privateChannel.SendMessageAsync(UserResponses.TicketPriorityChanged(guildOption, modUser, ticket, oldPriority, request.NewPriority));

      //TODO: Handle private messageContent privateChannel not found
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));
      }

      var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (ticketChannel is not null) {
        var newChName = request.NewPriority switch {
          TicketPriority.Normal => Const.NormalPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          TicketPriority.High => Const.HighPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          TicketPriority.Low => Const.LowPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          _ => ""
        };

        await ticketChannel.ModifyAsync(x => { x.Name = newChName; });
        await ticketChannel.SendMessageAsync(TicketResponses.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));
      }
      //TODO: Handle ticket privateChannel not found
    }, cancellationToken);
  }
}