using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Bot;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.UserInfo;
using NotFoundException = DSharpPlus.Exceptions.NotFoundException;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessChangePriorityHandler : IRequestHandler<ProcessChangePriorityCommand>
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
      var modUser = await _sender.Send(new GetDiscordUserInfoQuery(request.ModUserId), cancellationToken);

      try {
        var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
        var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
        await privateChannel.SendMessageAsync(UserResponses.TicketPriorityChanged(guildOption, modUser, ticket, oldPriority, request.NewPriority));
      }
      catch (NotFoundException) {
        //ignored
      }

      var logChannel = await _sender.Send(new GetDiscordLogChannelQuery(), cancellationToken);
      await logChannel.SendMessageAsync(LogResponses.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));

      try {
        var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
        var newChName = request.NewPriority switch {
          TicketPriority.Normal => Const.NormalPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          TicketPriority.High => Const.HighPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          TicketPriority.Low => Const.LowPriorityEmoji + string.Format(Const.TicketNameTemplate, ticket.OpenerUser?.Username.Trim()),
          _ => ""
        };

        await ticketChannel.ModifyAsync(x => { x.Name = newChName; });
        await ticketChannel.SendMessageAsync(TicketResponses.TicketPriorityChanged(modUser, ticket, oldPriority, request.NewPriority));
      }
      catch (NotFoundException) {
        //ignored
      }
    }, cancellationToken);
  }
}