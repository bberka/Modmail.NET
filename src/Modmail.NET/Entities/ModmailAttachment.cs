using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class ModmailAttachment
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDate { get; set; }

  public string Url { get; set; }
  public string ProxyUrl { get; set; }
  public byte[]? Content { get; set; }
  public long ReceivedUserId { get; set; }
  public long ReceivedChannelId { get; set; }
  public long ReceivedMessageId { get; set; }
}