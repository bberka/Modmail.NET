using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Metric.Static;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Database.Entities;

public class Option : IRegisterDateUtc, IUpdateDateUtc, IEntity
{
    public required ulong ServerId { get; init; }

    [MaxLength(DbLength.Name)]
    public required string Name { get; set; } = "Modmail";

    [MaxLength(DbLength.Url)]
    public required string IconUrl { get; set; } = "";

    [MaxLength(DbLength.Url)]
    public string? BannerUrl { get; set; }

    public required ulong LogChannelId { get; set; }
    public required ulong CategoryId { get; set; }
    public bool IsEnabled { get; set; } = true;

    [Range(-1, TicketConstants.TicketTimeoutMaxAllowedHours)]
    public long TicketTimeoutHours { get; set; } = -1;

    public bool TakeFeedbackAfterClosing { get; set; }
    public bool AlwaysAnonymous { get; set; }
    public bool PublicTranscripts { get; set; }
    public bool SendTranscriptLinkToUser { get; set; }

    [Range(MetricConstants.StatisticsCalculateDaysMin, MetricConstants.StatisticsCalculateDaysMax)]
    public int StatisticsCalculateDays { get; set; } = MetricConstants.DefaultStatisticsCalculateDays;

    public DateTime RegisterDateUtc { get; set; }
    public DateTime? UpdateDateUtc { get; set; }
}