using System.Text;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Jobs;
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
    var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
    ticket.ThrowIfNotOpen();

    var ticketType = await _dbContext.TicketTypes.FilterByNameOrKey(request.Type).FirstOrDefaultAsync(cancellationToken);
    if (ticketType is null)
      ticket.TicketTypeId = null;
    else
      ticket.TicketTypeId = ticketType.Id;

    _dbContext.Update(ticket);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    await Task.Run(async () => {
      //Don't await this task
      var userInfo = await _sender.Send(new GetDiscordUserInfoQuery(userId),
                                        cancellationToken);
      try {
        var ticketChannel = request.TicketChannel ?? await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
        await ticketChannel.SendMessageAsync(TicketBotMessages.Ticket.TicketTypeChanged(userInfo, ticketType));
      }
      catch (NotFoundException) { }


      if (ticket.BotTicketCreatedMessageInDmId != 0) {
        try {
          var privateChannelId = ticket.PrivateMessageChannelId;
          var privateChannel = request.PrivateChannel ?? await _bot.Client.GetChannelAsync(privateChannelId);
          var privateMessageWithComponent = request.PrivateMessageWithComponent ?? await privateChannel.GetMessageAsync(ticket.BotTicketCreatedMessageInDmId);
          var newEmbed = new DiscordEmbedBuilder(privateMessageWithComponent.Embeds[0]);
          if (ticketType is not null) {
            var sb = new StringBuilder();
            if (DiscordEmoji.TryFromUnicode(_bot.Client, ticketType.Emoji ?? string.Empty, out var emoji)) {
              sb.Append(emoji);
              sb.Append(' ');
            }

            sb.Append(ticketType.Name);
            newEmbed.AddField(Lang.TicketType.Translate(), sb.ToString());
          }

          await privateMessageWithComponent.ModifyAsync(x => {
            x.ClearComponents();
            x.ClearEmbeds();
            x.AddEmbed(newEmbed);
          });

          _ticketTypeSelectionTimeoutJob.RemoveMessage(privateMessageWithComponent.Id);
        }
        catch (NotFoundException) {
          //ignored
        }
      }
    }, cancellationToken);
  }
}