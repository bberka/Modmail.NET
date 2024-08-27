using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;

namespace Modmail.NET.Entities;

public class TicketMessageAttachment
{
  public Guid Id { get; set; }

  [MaxLength(DbLength.URL)]
  public string Url { get; set; }

  [MaxLength(DbLength.URL)]
  public string ProxyUrl { get; set; }

  public byte[]? Content { get; set; }

  public int? Height { get; set; }
  public int? Width { get; set; }

  [MaxLength(DbLength.FILE_NAME)]
  public string FileName { get; set; }

  public int FileSize { get; set; }

  [MaxLength(DbLength.MEDIA_TYPE)]
  public string MediaType { get; set; }

  public Guid TicketMessageId { get; set; }


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