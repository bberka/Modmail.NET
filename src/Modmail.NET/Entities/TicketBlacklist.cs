using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Modmail.NET.Entities;

[Index(nameof(DiscordUserInfoId), nameof(GuildOptionId), IsUnique = true)] 
public class TicketBlacklist
{
  [Key]
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public string? Reason { get; set; }
  public ulong DiscordUserInfoId { get; set; }
  public ulong GuildOptionId { get; set; }
  
  //FK
  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual GuildOption GuildOption { get; set; }
  
}