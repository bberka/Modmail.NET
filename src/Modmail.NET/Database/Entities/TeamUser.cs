using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TeamUser : IRegisterDateUtc, IEntity, IGuidId
{
    public required ulong UserId { get; init; }
    public required Guid TeamId { get; init; }
    public virtual Team? Team { get; set; }
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
}