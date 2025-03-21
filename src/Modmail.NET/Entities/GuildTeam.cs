using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class GuildTeam : IHasRegisterDate,
                                IHasUpdateDate,
                                IEntity
{
  public Guid Id { get; set; }
  public TeamPermissionLevel PermissionLevel { get; set; }

  [MaxLength(DbLength.NAME)]
  [Required]
  public required string Name { get; set; }

  public bool IsEnabled { get; set; } = true;
  public bool PingOnNewTicket { get; set; }
  public bool PingOnNewMessage { get; set; }

  //FK
  public List<GuildTeamMember> GuildTeamMembers { get; set; } = [];
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}