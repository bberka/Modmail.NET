using Ardalis.SmartEnum;

namespace Modmail.NET.Common.Static;

/// <summary>
///   AuthPolicy smart enum
/// </summary>
public sealed class AuthPolicy : SmartEnum<AuthPolicy>
{
  public static readonly AuthPolicy Support = new(nameof(Support), 1);
  public static readonly AuthPolicy Moderator = new(nameof(Moderator), 2);
  public static readonly AuthPolicy Admin = new(nameof(Admin), 3);
  public static readonly AuthPolicy Owner = new(nameof(Owner), 4);
  public static readonly AuthPolicy ManageTickets = new(nameof(ManageTickets), 6);
  public static readonly AuthPolicy ManageTicketTypes = new(nameof(ManageTicketTypes), 7);
  public static readonly AuthPolicy ManageTeams = new(nameof(ManageTeams), 8);
  public static readonly AuthPolicy ManageBlacklist = new(nameof(ManageBlacklist), 9);
  public static readonly AuthPolicy ManageHangfire = new(nameof(ManageHangfire), 10);

  //
  // public static readonly AuthPolicy ManageDiscordClient = new(nameof(ManageDiscordClient), 100);
  // public static readonly AuthPolicy ViewDashboardMetrics = new(nameof(ViewDashboardMetrics), 101);
  // public static readonly AuthPolicy ViewAnalytics = new(nameof(ViewAnalytics), 102);
  //
  // public static readonly AuthPolicy ViewTickets = new(nameof(ViewTickets), 202);
  // public static readonly AuthPolicy ViewTicketTranscript = new(nameof(ViewTicketTranscript), 203);
  // public static readonly AuthPolicy ViewTicketNotes = new(nameof(ViewTicketNotes), 204);
  // public static readonly AuthPolicy ViewTicketFeedbacks = new(nameof(ViewTicketFeedbacks), 205);
  // public static readonly AuthPolicy ViewTicketDetailMetrics = new(nameof(ViewTicketDetailMetrics), 207);
  //
  // public static readonly AuthPolicy BlockUser = new(nameof(BlockUser), 301);
  // public static readonly AuthPolicy RemoveBlock = new(nameof(RemoveBlock), 302);
  //
  // public static readonly AuthPolicy ViewTicketTypes = new(nameof(ViewTicketTypes), 400);
  // public static readonly AuthPolicy ManageTicketTypes = new(nameof(ManageTicketTypes), 401);
  //
  // public static readonly AuthPolicy ViewTeams = new(nameof(ViewTeams), 500);
  // public static readonly AuthPolicy CreateTeam = new(nameof(CreateTeam), 501);
  // public static readonly AuthPolicy ManageTeamsAndPermissions = new(nameof(ManageTeams), 502);
  //
  // public static readonly AuthPolicy ManageGuildOption = new(nameof(ManageGuildOption), 600);
  //
  // public static readonly AuthPolicy ManageHangfire = new(nameof(ManageHangfire), 700);


  private AuthPolicy(string name, int value) : base(name, value) { }
}