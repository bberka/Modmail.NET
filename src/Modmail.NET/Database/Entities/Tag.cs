using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Tag : IHasRegisterDate,
                   IHasUpdateDate,
                   IEntity,
                   IGuidId
{
  [MaxLength(DbLength.Tag)]
  public required string Shortcut { get; set; }

  public required string Content { get; set; }
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}