using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

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

  public static TicketMessageAttachment MapFrom(DiscordAttachment attachment, Guid ticketMessageId) {
    return new TicketMessageAttachment {
      Url = attachment.Url,
      ProxyUrl = attachment.ProxyUrl,
      TicketMessageId = ticketMessageId,
      Height = attachment.Height,
      Width = attachment.Width,
      FileName = attachment.FileName,
      FileSize = attachment.FileSize,
      MediaType = attachment.MediaType
    };
  }
}