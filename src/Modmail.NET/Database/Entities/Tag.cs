using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Tag : IHasRegisterDate,
                   IHasUpdateDate,
                   IEntity,
                   IGuidId
{
  [MaxLength(DbLength.TagName)]
  [Required]
  public required string Name { get; set; }

  [StringLength(DbLength.TagTitle)]
  public required string Title { get; set; } = null;

  [Required]
  public required string Content { get; set; }

  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}