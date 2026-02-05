using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketMessageAttachment : IEntity,
                                       IGuidId
{
  [MaxLength(DbLength.Url)]
  [Required]
  public required string Url { get; set; }

  [MaxLength(DbLength.Url)]
  [Required]
  public required string ProxyUrl { get; set; }

  public required int? Height { get; set; }
  public required int? Width { get; set; }

  [MaxLength(DbLength.FileName)]
  [Required]
  public required string FileName { get; set; }

  public required int FileSize { get; set; }

  [MaxLength(DbLength.MediaType)]
  [Required]
  public required string MediaType { get; set; }

  public required Guid TicketMessageId { get; set; }
  public Guid Id { get; set; }


  public static TicketMessageAttachment MapFrom(DiscordAttachment attachment, Guid ticketMessageId) {
    return new TicketMessageAttachment {
      Url = attachment.Url,
      ProxyUrl = attachment.ProxyUrl,
      TicketMessageId = ticketMessageId,
      Height = attachment.Height,
      Width = attachment.Width,
      FileName = attachment.FileName,
      FileSize = attachment.FileSize,
      MediaType = attachment.MediaType,
      Id = Guid.CreateVersion7()
    };
  }
}