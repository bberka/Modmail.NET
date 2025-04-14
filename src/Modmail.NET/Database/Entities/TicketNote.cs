using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketNote : IRegisterDateUtc,
                          IEntity,
                          IGuidId
{
  [MaxLength(DbLength.Note)]
  [Required]
  public required string Content { get; set; }

  public required Guid TicketId { get; set; }
  public required ulong UserId { get; set; }

  //FK
  public virtual UserInformation? User { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
}