using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Database;
using Modmail.NET.Entities;
using Modmail.NET.Exceptions;
using Modmail.NET.Features.Permission;
using Modmail.NET.Services;
using Modmail.NET.Utils;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessUserSentMessageHandler : IRequestHandler<ProcessUserSentMessageCommand>
{
  private readonly TicketAttachmentDownloadService _attachmentDownloadService;
  private readonly ModmailBot _bot;
  private readonly ModmailDbContext _dbContext;
  private readonly ISender _sender;

  public ProcessUserSentMessageHandler(ModmailDbContext dbContext,
                                       ModmailBot bot,
                                       ISender sender,
                                       TicketAttachmentDownloadService attachmentDownloadService) {
    _dbContext = dbContext;
    _bot = bot;
    _sender = sender;
    _attachmentDownloadService = attachmentDownloadService;
  }

  public async Task Handle(ProcessUserSentMessageCommand request, CancellationToken cancellationToken) {
    ArgumentNullException.ThrowIfNull(request.Message);
    await Task.Delay(50, cancellationToken); //wait for privateChannel creation process to finish
    var ticket = await _sender.Send(new GetTicketQuery(request.TicketId, MustBeOpen: true), cancellationToken);

    ticket.LastMessageDateUtc = UtilDate.GetNow();

    _dbContext.Update(ticket);
    var ticketMessage = TicketMessage.MapFrom(ticket.Id, request.Message, false);


    foreach (var attachment in ticketMessage.Attachments)
      await _attachmentDownloadService.Handle(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName));

    var mailChannel = await _bot.Client.GetChannelAsync(ticket.ModMessageChannelId);
    var permissions = await _sender.Send(new GetPermissionInfoQuery(), cancellationToken);
    var botMessage = await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(request.Message, ticketMessage.Attachments.ToArray(), permissions));

    ticketMessage.BotMessageId = botMessage.Id;
    _dbContext.Add(ticketMessage);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(Const.ProcessedReactionDiscordEmojiUnicode));
  }
}