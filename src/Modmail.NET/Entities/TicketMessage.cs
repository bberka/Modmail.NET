using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketMessage
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public ulong DiscordUserInfoId { get; set; }
  public string MessageContent { get; set; }

  //FK

  public virtual DiscordUserInfo DiscordUserInfo { get; set; }
  public virtual Guid TicketId { get; set; }
  public virtual List<TicketMessageAttachment> TicketMessageAttachments { get; set; }
}