using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Permission.Static;

namespace Modmail.NET.Database.Entities;

public class GuildTeam : IHasRegisterDate,
                         IHasUpdateDate,
                         IEntity,
                         IGuidId
{
  public required TeamPermissionLevel PermissionLevel { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public required string Name { get; set; }

  public bool IsEnabled { get; set; } = true;
  public bool PingOnNewTicket { get; set; }
  public bool PingOnNewMessage { get; set; }

  public bool AllowAccessToWebPanel { get; set; } = false;

  //FK
  public List<GuildTeamMember> GuildTeamMembers { get; set; } = [];
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}