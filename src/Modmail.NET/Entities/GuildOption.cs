using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public class GuildOption : IHasRegisterDate,
                           IHasUpdateDate,
                           IEntity
{
  public ulong GuildId { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public string Name { get; set; } = "Modmail";

  [MaxLength(DbLength.Url)]
  public string IconUrl { get; set; } = "";

  [MaxLength(DbLength.Url)]
  public string BannerUrl { get; set; }

  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  [Range(Const.TicketTimeoutMinAllowedHours, Const.TicketTimeoutMaxAllowedHours)]
  public long TicketTimeoutHours { get; set; } = Const.DefaultTicketTimeoutHours;

  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }
  public bool AlwaysAnonymous { get; set; } = false;
  public TeamPermissionLevel ManageTicketMinAccessLevel { get; set; } = TeamPermissionLevel.Moderator;
  public TeamPermissionLevel ManageTeamsMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageBlacklistMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageTicketTypeMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;
  public TeamPermissionLevel ManageHangfireMinAccessLevel { get; set; } = TeamPermissionLevel.Admin;

  public DateTime RegisterDateUtc { get; set; }

  public DateTime? UpdateDateUtc { get; set; }
}