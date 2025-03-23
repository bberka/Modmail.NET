using Ardalis.SmartEnum;

namespace Modmail.NET.Web.Blazor.Static;

/// <summary>
/// AuthPolicy smart enum 
/// </summary>
public sealed class AuthPolicy : SmartEnum<AuthPolicy>
{
  private AuthPolicy(string name, int value) : base(name, value) { }
  public static readonly AuthPolicy Support = new(nameof(Support), 1);
  public static readonly AuthPolicy Moderator = new(nameof(Moderator), 2);
  public static readonly AuthPolicy Admin = new(nameof(Admin), 3);
  public static readonly AuthPolicy Owner = new(nameof(Owner), 4);
  public static readonly AuthPolicy ManageOptions = new(nameof(ManageOptions), 5);
  public static readonly AuthPolicy ManageTickets = new(nameof(ManageTickets), 6);
  public static readonly AuthPolicy ManageTicketTypes = new(nameof(ManageTicketTypes), 7);
  public static readonly AuthPolicy ManageTeams = new(nameof(ManageTeams), 8);
  public static readonly AuthPolicy ManageBlacklist = new(nameof(ManageBlacklist), 9);
  public static readonly AuthPolicy ManageHangfire = new(nameof(ManageHangfire), 10);
}