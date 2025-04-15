using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
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
	public required string Content { get; set; }

	public required Guid TicketId { get; set; }
	public required ulong UserId { get; set; }
	public virtual UserInformation? User { get; set; }
}