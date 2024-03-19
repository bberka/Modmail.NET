using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketMessageAttachment
{
  [Key]
  public Guid Id { get; set; }

  public string Url { get; set; }
  public string ProxyUrl { get; set; }
  public byte[]? Content { get; set; }

  public int? Height { get; set; }
  public int? Width { get; set; }
  public string FileName { get; set; }
  public int FileSize { get; set; }
  public string MediaType { get; set; }


  public Guid TicketMessageId { get; set; }

  //FK
  public virtual TicketMessage TicketMessage { get; set; }
}