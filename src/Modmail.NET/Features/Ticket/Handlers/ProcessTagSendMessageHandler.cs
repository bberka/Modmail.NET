using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Common.Static;
using Modmail.NET.Common.Utils;
using Modmail.NET.Database;
using Modmail.NET.Database.Extensions;
using Modmail.NET.Features.Server.Queries;
using Modmail.NET.Features.Ticket.Commands;
using Modmail.NET.Features.User.Queries;
using Modmail.NET.Language;
using TicketMessage = Modmail.NET.Database.Entities.TicketMessage;

namespace Modmail.NET.Features.Ticket.Handlers;

public class ProcessTagSendMessageHandler : IRequestHandler<ProcessTagSendMessageCommand>
{
	private readonly ModmailBot _bot;
	private readonly ModmailDbContext _dbContext;
	private readonly ISender _sender;

	public ProcessTagSendMessageHandler(ISender sender,
	                                    ModmailBot bot,
	                                    ModmailDbContext dbContext) {
		_sender = sender;
		_bot = bot;
		_dbContext = dbContext;
	}

	public async ValueTask<Unit> Handle(ProcessTagSendMessageCommand request, CancellationToken cancellationToken) {
		var guildOption = await _sender.Send(new GetOptionQuery(), cancellationToken);

		var ticket = await _dbContext.Tickets
		                             .FilterActive()
		                             .FilterById(request.TicketId)
		                             .FirstOrDefaultAsync(cancellationToken) ?? throw new ModmailBotException(Lang.TicketNotFound);

		ticket.LastMessageDateUtc = UtilDate.GetNow();
		_dbContext.Update(ticket);

		var tag = await _dbContext.Tags
		                          .FilterById(request.TagId)
		                          .FirstOrDefaultAsync(cancellationToken) ?? throw new ModmailBotException(Lang.TagNotFound);


		var ticketMessage = new TicketMessage {
			SenderUserId = request.UserId,
			MessageDiscordId = 0,
			TicketId = request.TicketId,
			SentByMod = true,
			TagId = tag.Id
		};

		var author = await _sender.Send(new GetDiscordUserInfoQuery(request.UserId), cancellationToken);


		var embed = new DiscordEmbedBuilder()
		            .WithDescription(tag.Content)
		            .WithGuildInfoFooter()
		            .WithCustomTimestamp()
		            .WithColor(ModmailColors.TagReceivedColor);
		if (!string.IsNullOrEmpty(tag.Title)) embed.WithTitle(tag.Title);
		if (!(ticket.Anonymous || guildOption.AlwaysAnonymous))
			embed.WithUserAsAuthor(author);
		var msg = new DiscordMessageBuilder();
		msg.AddEmbed(embed);
		var privateChannel = await _bot.Client.GetChannelAsync(ticket.PrivateMessageChannelId);
		var botMessage = await privateChannel.SendMessageAsync(embed);

		ticketMessage.BotMessageId = botMessage.Id;
		_dbContext.Add(ticketMessage);

		var affected = await _dbContext.SaveChangesAsync(cancellationToken);
		if (affected == 0) throw new DbInternalException();
		return Unit.Value;
	}
}