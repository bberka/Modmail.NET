using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class TeamUser : IRegisterDateUtc,
                        IEntity,
                        IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public required ulong UserId { get; set; }
	public required Guid TeamId { get; set; }
	public virtual Team? Team { get; set; }
}