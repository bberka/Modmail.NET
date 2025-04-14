using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class TeamUser : IRegisterDateUtc,
                        IEntity,
                        IGuidId
{
  public required ulong UserId { get; set; }

  public required Guid TeamId { get; set; }

  //FK
  public virtual Team? Team { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}