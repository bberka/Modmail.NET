using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketMessageAttachment : IEntity, IRegisterDateUtc, IGuidId
{
    public required Guid TicketMessageId { get; init; }

    [MaxLength(DbLength.Url)]
    [Required]
    public required string Url { get; init; }

    [MaxLength(DbLength.Url)]
    [Required]
    public string? ProxyUrl { get; init; }

    public required int? Height { get; init; }
    public required int? Width { get; init; }

    [MaxLength(DbLength.FileName)]
    [Required]
    public required string FileName { get; init; }

    public required int FileSize { get; init; }

    [MaxLength(DbLength.MediaType)]
    [Required]
    public required string MediaType { get; init; }

    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }

    public static TicketMessageAttachment MapFrom(DiscordAttachment attachment, Guid ticketMessageId)
    {
        var isUrlValid = Uri.TryCreate(attachment.Url, UriKind.Absolute, out _);
        var isProxyUrlValid = Uri.TryCreate(attachment.ProxyUrl, UriKind.Absolute, out _);
        if (!isUrlValid || !isProxyUrlValid) throw new ArgumentException("Invalid URL or Proxy URL");

        return new TicketMessageAttachment
        {
            Url = attachment.Url ?? throw new ArgumentNullException(nameof(attachment.Url)),
            ProxyUrl = attachment.ProxyUrl,
            TicketMessageId = ticketMessageId,
            Height = attachment.Height,
            Width = attachment.Width,
            FileName = attachment.FileName ?? throw new ArgumentNullException(nameof(attachment.FileName)),
            FileSize = attachment.FileSize,
            MediaType = attachment.MediaType ?? throw new ArgumentNullException(nameof(attachment.MediaType)),
            Id = Guid.CreateVersion7()
        };
    }
}