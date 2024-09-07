using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public sealed class TicketNote
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;

  [MaxLength(DbLength.NOTE)]
  public required string Content { get; set; }

  public Guid TicketId { get; set; }
  public ulong DiscordUserId { get; set; }
}