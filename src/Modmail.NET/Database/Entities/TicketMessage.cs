using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Exceptions;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Ticket.Static;
using Modmail.NET.Language;

namespace Modmail.NET.Database.Entities;

public class TicketMessage : IRegisterDateUtc,
                             IGuidId,
                             IEntity
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public ulong BotMessageId { get; set; }
	public required ulong SenderUserId { get; init; }

	[MaxLength(DbLength.Message)]
	public string? MessageContent { get; set; }

	public required ulong MessageDiscordId { get; init; }
	public required Guid TicketId { get; init; }
	public required bool SentByMod { get; init; }
	public Guid? TagId { get; init; }
	public TicketMessageChangeStatus ChangeStatus { get; set; } = TicketMessageChangeStatus.None;
	public virtual ICollection<TicketMessageAttachment> Attachments { get; set; } = [];
	public virtual ICollection<TicketMessageHistory> History { get; set; } = [];
	public virtual Tag? Tag { get; set; }

	public static TicketMessage MapFrom(Guid ticketId, DiscordMessage message, bool sentByMod) {
		var id = Guid.CreateVersion7();
		return new TicketMessage {
			Id = id,
			SenderUserId = message.Author?.Id ?? throw new ModmailBotException(Lang.InvalidUser),
			MessageContent = message.Content,
			TicketId = ticketId,
			Attachments = message.Attachments.Select(x => TicketMessageAttachment.MapFrom(x, id)).ToList(),
			MessageDiscordId = message.Id,
			RegisterDateUtc = UtilDate.GetNow(),
			SentByMod = sentByMod
		};
	}
}