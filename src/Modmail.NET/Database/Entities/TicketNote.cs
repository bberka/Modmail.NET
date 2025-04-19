using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketNote : IRegisterDateUtc,
                          IEntity,
                          IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }

	[MaxLength(DbLength.Note)]
	[Required]
	public required string Content { get; init; }

	public required Guid TicketId { get; init; }
	public required ulong UserId { get; init; }
	public virtual UserInformation? User { get; set; }
}