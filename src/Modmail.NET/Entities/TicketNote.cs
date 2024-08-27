using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketNote
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  [MaxLength(DbLength.NOTE)]
  public string Content { get; set; }

  public Guid TicketId { get; set; }
  public ulong DiscordUserId { get; set; }
}