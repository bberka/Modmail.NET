using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Team : IRegisterDateUtc,
                    IUpdateDateUtc,
                    IEntity,
                    IGuidId
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public DateTime? UpdateDateUtc { get; set; }

	[MaxLength(DbLength.Name)]
	[Required]
	public required string Name { get; set; }

	public bool PingOnNewTicket { get; set; }
	public bool PingOnNewMessage { get; set; }
	public bool SuperUserTeam { get; init; }
	public virtual ICollection<TeamUser> Users { get; set; } = [];
	public virtual ICollection<TeamPermission> Permissions { get; set; } = [];
}