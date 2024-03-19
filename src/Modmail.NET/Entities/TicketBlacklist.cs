using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketBlacklist
{
  [Key]
  public Guid Id { get; set; }
  
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime EndDateUtc { get; set; } = DateTime.UtcNow;
  
  public string? Reason { get; set; }
  
  public ulong DiscordUserInfoId { get; set; }
  
  
  //FK

  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  
}