using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Modmail.NET.Entities;

public class TicketNote
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string Content { get; set; }
  public Guid TicketId { get; set; }

  [ForeignKey(nameof(DiscordUserInfo))]
  public ulong DiscordUserId { get; set; }

  public virtual Ticket Ticket { get; set; }
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
}