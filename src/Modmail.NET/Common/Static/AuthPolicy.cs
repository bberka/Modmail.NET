using Ardalis.SmartEnum;

namespace Modmail.NET.Common.Static;

/// <summary>
///     AuthPolicy smart enum
/// </summary>
public sealed class AuthPolicy : SmartEnum<AuthPolicy>
{
    public static readonly AuthPolicy SuperUser = new(nameof(SuperUser), 1, "Full access");

    public static readonly AuthPolicy ManageDiscordClient = new(nameof(ManageDiscordClient), 10, "Access to discord start stop etc.");
    public static readonly AuthPolicy ViewAnalytics = new(nameof(ViewAnalytics), 30, "View detailed analytics");

    public static readonly AuthPolicy ViewTicketFeedbacks = new(nameof(ViewTicketFeedbacks), 40, "View user ticket feedbacks");

    public static readonly AuthPolicy ManageBlacklist = new(nameof(ManageBlacklist), 50, "Block, unblock users from ticket system");

    public static readonly AuthPolicy ManageTicketTypes = new(nameof(ManageTicketTypes), 60, "Create, update or delete ticket types");

    public static readonly AuthPolicy ManageTags = new(nameof(ManageTags), 70, "Create, update or delete tags");

    public static readonly AuthPolicy ManageAccessPermissions = new(nameof(ManageAccessPermissions), 80, "Manage to access permissions and teams");

    public static readonly AuthPolicy ManageOptions = new(nameof(ManageOptions), 90, "Update and change options");

    public static readonly AuthPolicy ManageHangfire = new(nameof(ManageHangfire), 100, "Access to hangfire dashboard");


    private AuthPolicy(
        string name,
        int value,
        string? description = null
    ) : base(name, value)
    {
        Description = description;
    }

    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    public string? Description { get; set; }
}