using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Blacklist : IRegisterDateUtc,
                         IEntity,
                         IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }

	[MaxLength(DbLength.Reason)]
	public required string Reason { get; set; }

	public required ulong UserId { get; set; }
	public required ulong AuthorUserId { get; set; }
	public virtual UserInformation? User { get; set; }
	public virtual UserInformation? AuthorUser { get; set; }
}