using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class GuildTeamMember : IHasRegisterDate,
                               IEntity,
                               IGuidId
{
  public required ulong Key { get; set; }
  public required TeamMemberDataType Type { get; set; }
  public required Guid GuildTeamId { get; set; }
  public GuildTeam GuildTeam { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}