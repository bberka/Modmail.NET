using Modmail.NET.Database.Entities;
using Modmail.NET.Features.DiscordCommands.Checks.Attributes;

namespace Modmail.NET.Features.Tag.Commands;

[RequireModmailPermission(nameof(AuthPolicy.ManageTags))]
public sealed record ProcessUpdateTagCommand : IRequest<Database.Entities.Tag>, IPermissionCheck
{
    private string _name = string.Empty;
    public required Guid TagId { get; set; }

    public required string Name
    {
        get => _name;
        set => _name = Database.Entities.Tag.GetTagName(value);
    }

    public required bool IncludeAuthorWhenTicketChannel { get; set; }

    public required Embed Embed { get; set; }
    public required ulong AuthorizedUserId { get; set; }
}