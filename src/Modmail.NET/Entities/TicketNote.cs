using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketNote
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string Content { get; set; }
  public Guid TicketId { get; set; }
  public ulong DiscordUserInfoId { get; set; }
  public string Username { get; set; }
  public virtual Ticket Ticket { get; set; }

  
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
}