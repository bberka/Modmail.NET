using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.Ticket.Services;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Features.User.Queries;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

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

	public async ValueTask<Unit> Handle(ProcessModSendMessageCommand request, CancellationToken cancellationToken) {
		ArgumentNullException.ThrowIfNull(request.Message);
		var author = await _sender.Send(new GetDiscordUserInfoQuery(request.AuthorizedUserId), cancellationToken);

		var ticket = await _dbContext.Tickets.FindAsync([request.TicketId], cancellationToken) ?? throw new NullReferenceException(nameof(Ticket));
		ticket.ThrowIfNotOpen();
		ticket.LastMessageDateUtc = UtilDate.GetNow();

		_dbContext.Update(ticket);


		var option = await _sender.Send(new GetOptionQuery(), cancellationToken);

		var ticketMessage = TicketMessage.MapFrom(request.TicketId, request.Message, true);

		foreach (var attachment in ticketMessage.Attachments)
			await _attachmentDownloadService.Handle(attachment.Id, attachment.Url, Path.GetExtension(attachment.FileName));

		var anonymous = option.AlwaysAnonymous || ticket.Anonymous;
		var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);

		var msg = new DiscordMessageBuilder();
		msg.AddAttachments(ticketMessage.Attachments.ToArray());
		var embed = new DiscordEmbedBuilder()
		            .WithDescription(request.Message.Content)
		            .WithServerInfoFooter(option)
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.MessageReceivedColor);

		if (!anonymous) embed.WithUserAsAuthor(author);
		msg.AddEmbed(embed);

		var botMessage = await privateChannel.SendMessageAsync(embed);

		ticketMessage.BotMessageId = botMessage.Id;
		await _dbContext.AddAsync(ticketMessage, cancellationToken);

		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();

		await request.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(TicketConstants.ProcessedReactionDiscordEmojiUnicode));
		return Unit.Value;
	}
}