using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketMessageHistory : IRegisterDateUtc,
                                    IGuidId,
                                    IEntity
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }

	[MaxLength(DbLength.Message)]
	public required string? MessageContentBefore { get; init; }

	[MaxLength(DbLength.Message)]
	public required string? MessageContentAfter { get; init; }

	public required Guid TicketMessageId { get; init; }
	public virtual TicketMessage? TicketMessage { get; set; }
}