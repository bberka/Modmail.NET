using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class GuildTeamMember : IHasRegisterDate,
                               IEntity,
                               IGuidId
{
  public ulong Key { get; set; }
  public TeamMemberDataType Type { get; set; }
  public Guid GuildTeamId { get; set; }
  public GuildTeam GuildTeam { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}