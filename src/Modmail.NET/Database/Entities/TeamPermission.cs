using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TeamPermission : IRegisterDateUtc, IEntity, IGuidId
{
    public required AuthPolicy AuthPolicy { get; init; }
    public required Guid TeamId { get; init; }
    public virtual Team? Team { get; set; }
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
}