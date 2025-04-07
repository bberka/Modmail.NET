using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Metric.Static;
using Modmail.NET.Features.Permission.Static;
using Modmail.NET.Features.Ticket.Static;

namespace Modmail.NET.Database.Entities;

public class GuildOption : IHasRegisterDate,
                           IHasUpdateDate,
                           IEntity
{
  public required ulong GuildId { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public required string Name { get; set; } = "Modmail";

  [MaxLength(DbLength.Url)]
  public required string IconUrl { get; set; } = "";

  [MaxLength(DbLength.Url)]
  public required string BannerUrl { get; set; }

  public required ulong LogChannelId { get; set; }

  public required ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  [Range(TicketConstants.TicketTimeoutMinAllowedHours, TicketConstants.TicketTimeoutMaxAllowedHours)]
  public long TicketTimeoutHours { get; set; } = -1;

  public bool TakeFeedbackAfterClosing { get; set; }
  public bool AlwaysAnonymous { get; set; }
  public bool PublicTranscripts { get; set; }
  public bool SendTranscriptLinkToUser { get; set; }

  [Range(-1, TicketConstants.TicketDataDeleteWaitDaysMax)]
  public int TicketDataDeleteWaitDays { get; set; } = -1;

  [Range(-1, MetricConstants.StatisticsCalculateDaysMax)]
  public int StatisticsCalculateDays { get; set; } = MetricConstants.DefaultStatisticsCalculateDays;

  public TeamPermissionLevel ManageTicketMinAccessLevel { get; set; } = TeamPermissionLevel.Moderator;
  public TeamPermissionLevel ManageTeamsMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageBlacklistMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageTicketTypeMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageHangfireMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;

  public DateTime RegisterDateUtc { get; set; }

  public DateTime? UpdateDateUtc { get; set; }
}