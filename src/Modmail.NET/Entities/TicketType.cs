using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketType
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? UpdateDateUtc { get; set; }
  public string Key { get; set; }
  public string Name { get; set; }
  public string? Emoji { get; set; }
  public string? Description { get; set; }
  public int Order { get; set; }
}