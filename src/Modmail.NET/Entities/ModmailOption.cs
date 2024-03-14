using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class ModmailOption
{
  [Key]
  public Guid Id { get; set; }
  public long GuildId { get; set; }
  public long LogChannelId { get; set; }
  public long CategoryId { get; set; }
  public bool IsListenPrivateMessages { get; set; } = true;
}