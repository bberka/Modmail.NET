using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Features.TicketType;
using Modmail.NET.Features.UserInfo;
using Modmail.NET.Jobs;

namespace Modmail.NET.Features.Ticket.Handlers;

public sealed class ProcessChangeTicketTypeHandler : IRequestHandler<ProcessChangeTicketTypeCommand>
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
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketResponses.TicketTypeChanged(userInfo, ticketType));

      //TODO: Handle mail channel not found
      var privateChannelId = ticket.PrivateMessageChannelId;
      var privateChannel = request.PrivateChannel ?? await _bot.Client.GetChannelAsync(privateChannelId);
      if (privateChannel is not null)
        // await privateChannel.SendMessageAsync(UserResponses.TicketTypeChanged(ticketType));
        if (ticket.BotTicketCreatedMessageInDmId != 0) {
          var privateMessageWithComponent = request.PrivateMessageWithComponent ?? await privateChannel.GetMessageAsync(ticket.BotTicketCreatedMessageInDmId);
          if (privateMessageWithComponent is not null) {
            var newEmbed = new DiscordEmbedBuilder(privateMessageWithComponent.Embeds[0]);
            if (ticketType is not null) {
              var emoji = DiscordEmoji.FromUnicode(_bot.Client, ticketType.Emoji);
              var typeName = ticketType.Name;
              var str = $"{emoji} {typeName}";
              newEmbed.AddField(LangKeys.TICKET_TYPE.GetTranslation(), str);
            }

            await privateMessageWithComponent.ModifyAsync(x => {
              x.ClearComponents();
              x.ClearEmbeds();
              x.AddEmbed(newEmbed);
            });

            _ticketTypeSelectionTimeoutJob.RemoveMessage(privateMessageWithComponent.Id);
          }
        }

      //TODO: Handle private channel not found
      var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await _bot.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketTypeChanged(ticket, ticketType));
      }
    }, cancellationToken);
  }
}