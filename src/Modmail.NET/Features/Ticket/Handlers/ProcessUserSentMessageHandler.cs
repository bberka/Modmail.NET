using DSharpPlus.Entities;
using MediatR;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Features.Permission.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Helpers;
using Modmail.NET.Features.Ticket.Queries;
using Modmail.NET.Features.Ticket.Services;
using Modmail.NET.Features.Ticket.Static;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

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
    var pingOnNewTicket = permissions.Where(x => x.PingOnNewMessage).ToArray();

    var msg = TicketBotMessages.Ticket.MessageReceived(request.Message, ticketMessage.Attachments.ToArray());
    msg.WithContent(UtilMention.GetMentionsMessageString(pingOnNewTicket));
    var botMessage = await mailChannel.SendMessageAsync(msg);

    ticketMessage.BotMessageId = botMessage.Id;
    _dbContext.Add(ticketMessage);
    var affected = await _dbContext.SaveChangesAsync(cancellationToken);
    if (affected == 0) throw new DbInternalException();

    await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(TicketConstants.ProcessedReactionDiscordEmojiUnicode));
  }
}