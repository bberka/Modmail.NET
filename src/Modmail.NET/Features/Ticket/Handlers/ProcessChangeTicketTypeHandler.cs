using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Jobs;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;
using NotFoundException = DSharpPlus.Exceptions.NotFoundException;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessChangeTicketTypeHandler : IRequestHandler<ProcessChangeTicketTypeCommand>
{
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;
  private readonly TicketTypeSelectionTimeoutJob _ticketTypeSelectionTimeoutJob;

  public ProcessChangeTicketTypeHandler(ModmailBot bot,
                                        ModmailDbContext dbContext,
                                        ISender sender,
                                        TicketTypeSelectionTimeoutJob ticketTypeSelectionTimeoutJob) {
    _bot = bot;
    _dbContext = dbContext;
    _sender = sender;
    _ticketTypeSelectionTimeoutJob = ticketTypeSelectionTimeoutJob;
  }

  public async Task Handle(ProcessChangeTicketTypeCommand request, CancellationToken cancellationToken) {
    var userId = request.UserId == 0
                   ? _bot.Client.CurrentUser.Id
                   : request.UserId;

    //TODO: maybe add removal of embeds for the message to keep getting called if ticket is closed
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);


    var ticketType = await _sender.Send(new GetTicketTypeBySearchQuery(request.Type,
                                                                       true), cancellationToken);

    if (ticketType is null)
      ticket.TicketTypeId = null;
    else
      ticket.TicketTypeId = ticketType.Id;

    _dbContext.Tickets.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    await Task.Run(async () => {
      //Don't await this task
      var userInfo = await _sender.Send(new GetDiscordUserInfoQuery(userId),
                                        cancellationToken);
      var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketBotMessages.Ticket.TicketTypeChanged(userInfo, ticketType));

      if (ticket.BotTicketCreatedMessageInDmId != 0) {
        try {
          var privateChannelId = ticket.PrivateMessageChannelId;
          var privateChannel = request.PrivateChannel ?? await _bot.Client.GetChannelAsync(privateChannelId);
          var privateMessageWithComponent = request.PrivateMessageWithComponent ?? await privateChannel.GetMessageAsync(ticket.BotTicketCreatedMessageInDmId);
          if (privateMessageWithComponent is not null) {
            var newEmbed = new DiscordEmbedBuilder(privateMessageWithComponent.Embeds[0]);
            if (ticketType is not null) {
              var emoji = DiscordEmoji.FromUnicode(_bot.Client, ticketType.Emoji);
              var typeName = ticketType.Name;
              var str = $"{emoji} {typeName}";
              newEmbed.AddField(LangKeys.TicketType.GetTranslation(), str);
            }

            await privateMessageWithComponent.ModifyAsync(x => {
              x.ClearComponents();
              x.ClearEmbeds();
              x.AddEmbed(newEmbed);
            });

            _ticketTypeSelectionTimeoutJob.RemoveMessage(privateMessageWithComponent.Id);
          }
        }
        catch (NotFoundException) {
          //ignored
        }
      }
    }, cancellationToken);
  }
}