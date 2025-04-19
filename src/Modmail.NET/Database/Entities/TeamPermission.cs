using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

[Index(nameof(TeamId), nameof(AuthPolicy), IsUnique = true)]
public class TeamPermission : IRegisterDateUtc,
                              IEntity,
                              IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public required AuthPolicy AuthPolicy { get; init; }
	public required Guid TeamId { get; init; }
	public virtual Team? Team { get; set; }
}