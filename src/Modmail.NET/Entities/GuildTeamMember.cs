using System.ComponentModel.DataAnnotations;
using Modmail.NET.Static;

namespace Modmail.NET.Entities;

public class GuildTeamMember
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDate { get; set; }
  public DateTime? UpdateDate { get; set; }

  public ulong Key { get; set; }

  public TeamMemberDataType Type { get; set; }

  public Guid GuildTeamId { get; set; }
  public virtual GuildTeam GuildTeam { get; set; }
}