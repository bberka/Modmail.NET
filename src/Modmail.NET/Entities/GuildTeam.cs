using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class GuildTeam : IHasRegisterDate,
                         IHasUpdateDate,
                         IEntity,
                         IGuidId
{
  public Guid Id { get; set; }
  public TeamPermissionLevel PermissionLevel { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public required string Name { get; set; }

  public bool IsEnabled { get; set; } = true;
  public bool PingOnNewTicket { get; set; }
  public bool PingOnNewMessage { get; set; }

  public bool AllowAccessToWebPanel { get; set; } = false;

  //FK
  public List<GuildTeamMember> GuildTeamMembers { get; set; } = [];
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}