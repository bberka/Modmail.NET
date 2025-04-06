using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Guild;
using Modmail.NET.Services;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessModSendMessageHandler : IRequestHandler<ProcessModSendMessageCommand>
{
  private readonly TicketAttachmentDownloadService _attachmentDownloadService;
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessModSendMessageHandler(ISender sender,
                                      ModmailBot bot,
                                      ModmailDbContext dbContext,
                                      TicketAttachmentDownloadService attachmentDownloadService) {
    _sender = sender;
    _bot = bot;
    _dbContext = dbContext;
    _attachmentDownloadService = attachmentDownloadService;
  }

  public async Task Handle(ProcessModSendMessageCommand request, CancellationToken cancellationToken) {
    ArgumentNullException.ThrowIfNull(request.ModUser);
    ArgumentNullException.ThrowIfNull(request.Message);
    ArgumentNullException.ThrowIfNull(request.Channel);
    ArgumentNullException.ThrowIfNull(request.Guild);


    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);
    ticket.LastMessageDateUtc = UtilDate.GetNow();

    _dbContext.Update(ticket);


    var guildOption = await _sender.Send(new GetGuildOptionQuery(false), cancellationToken);

    var ticketMessage = TicketMessage.MapFrom(request.TicketId, request.Message, true);

    foreach (var attachment in ticketMessage.Attachments)
      await _attachmentDownloadService.Handle(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName));

    var anonymous = guildOption.AlwaysAnonymous || ticket.Anonymous;
    var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
    var embed = UserResponses.MessageReceived(request.Message, ticketMessage.Attachments.ToArray(), anonymous);
    var botMessage = await privateChannel.SendMessageAsync(embed);

    ticketMessage.BotMessageId = botMessage.Id;
    await _dbContext.AddAsync(ticketMessage, cancellationToken);

    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Const.ProcessedReactionDiscordEmojiUnicode));
  }
}