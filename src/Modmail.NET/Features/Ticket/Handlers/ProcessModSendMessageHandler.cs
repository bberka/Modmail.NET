using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Queues;
using Modmail.NET.Utils;

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

    var ticketMessage = TicketMessage.MapFrom(request.TicketId, request.Message, true);

    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);
    ticket.LastMessageDateUtc = UtilDate.GetNow();

    _dbContext.Update(ticket);
    await _dbContext.AddAsync(ticketMessage, cancellationToken);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);


    foreach (var attachment in ticketMessage.Attachments)
      TicketAttachmentDownloadQueueHandler.Enqueue(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName));


    _ = Task.Run(async () => {
      var anonymous = guildOption.AlwaysAnonymous || ticket.Anonymous;
      var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
      var embed = UserResponses.MessageReceived(request.Message, ticketMessage.Attachments.ToArray(), anonymous);
      await privateChannel.SendMessageAsync(embed);

      var bot = ServiceLocator.GetModmailBot();
      await request.Message.CreateReactionAsync(DiscordEmoji.FromName(bot.Client, Const.ProcessedReactionDiscordEmojiString, false));
    }, cancellationToken);
  }
}