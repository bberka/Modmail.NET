using System.ComponentModel.DataAnnotations;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class GuildTeam
{
  [Key]
  public Guid Id { get; set; }

  public TeamPermissionLevel PermissionLevel { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }

  public string Name { get; set; }

  public bool IsEnabled { get; set; } = true;


  public ulong GuildOptionId { get; set; }

  public virtual GuildOption GuildOption { get; set; }

  public virtual List<GuildTeamMember> GuildTeamMembers { get; set; }
}