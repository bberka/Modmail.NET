using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Tag.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTags))]
public sealed record ProcessCreateTagCommand : IRequest, IPermissionCheck
{
    private string _name = string.Empty;

    public required string Name
    {
        get => _name;
        set => _name = Database.Entities.Tag.GetTagName(value);
    }

    public required Embed Embed { get; set; }

    public required bool IncludeAuthorWhenTicketChannel { get; set; }

    public required ulong AuthorizedUserId { get; set; }
    // public required bool IncludeAuthorWhenTicketChannel { get; set; }
    // public required string Title { get; set; }
    // public required string Description { get; set; }
    // public required string? Color { get; set; }
    // public required string? ImageUrl { get; set; }
    // public required string? ThumbnailUrl { get; set; }
    // public required bool WithTimestamp { get; set; }
    // public required bool WithServerInfoFooter { get; set; }
    // public required ulong? AuthorId { get; set; }
}