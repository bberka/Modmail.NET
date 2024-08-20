using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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