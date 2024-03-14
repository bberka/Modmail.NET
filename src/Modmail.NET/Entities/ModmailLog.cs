using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class ModmailLog
{
  [Key]
  public Guid Id { get; set; }
  public DateTime RegisterDate { get; set; }
  public DateTime? ClosedDate { get; set; }
  public long DiscordUserId { get; set; }
  public long PrivateMessageChannelId { get; set; }
  public long ModMessageChannelId { get; set; }
  public long GuildId { get; set; }
}