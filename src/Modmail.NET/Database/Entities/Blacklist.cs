using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Blacklist : IRegisterDateUtc,
                         IEntity,
                         IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }

	[StringLength(DbLength.Reason)]
	public required string Reason { get; init; }

	public required ulong UserId { get; init; }
	public required ulong AuthorUserId { get; init; }
	public virtual UserInformation? User { get; set; }
	public virtual UserInformation? AuthorUser { get; set; }
}