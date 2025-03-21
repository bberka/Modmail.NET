using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class GuildOption : IHasRegisterDate,
                                  IHasUpdateDate,
                                  IEntity
{
  public ulong GuildId { get; set; }

  [MaxLength(DbLength.NAME)]
  [Required]
  public string Name { get; set; } = "Modmail";

  [MaxLength(DbLength.URL)]
  public string IconUrl { get; set; } = "";

  [MaxLength(DbLength.URL)]
  public string BannerUrl { get; set; }

  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  public bool IsSensitiveLogging { get; set; } = true;

  [Range(Const.TICKET_TIMEOUT_MIN_ALLOWED_HOURS, Const.TICKET_TIMEOUT_MAX_ALLOWED_HOURS)]
  public long TicketTimeoutHours { get; set; } = Const.DEFAULT_TICKET_TIMEOUT_HOURS;

  public bool IsEnableDiscordChannelLogging { get; set; } = true;
  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }
  public bool AlwaysAnonymous { get; set; } = false;
  public bool DisableBlacklistSlashCommands { get; set; } = false;
  public bool DisableTicketSlashCommands { get; set; } = false;

  public bool AllowUsersToCloseTickets { get; set; } = false;

  [Precision(2)]
  public double AvgResponseTimeMinutes { get; set; }

  [Precision(2)]
  public double AvgTicketsClosePerDay { get; set; }

  [Precision(2)]
  public double AvgTicketsOpenPerDay { get; set; }

  public DateTime RegisterDateUtc { get; set; }

  public DateTime? UpdateDateUtc { get; set; }
}