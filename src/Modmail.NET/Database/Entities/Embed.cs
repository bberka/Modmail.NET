using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Common.Extensions;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Embed : IEntity, IRegisterDateUtc, IUpdateDateUtc, IGuidId
{
    [StringLength(DbLength.Title)]
    public required string Title { get; set; }

    [StringLength(DbLength.Message)]
    public required string Description { get; set; }

    [StringLength(DbLength.DiscordColor)]
    public required string? Color { get; set; }

    [StringLength(DbLength.Url)]
    [Url]
    public required string? ImageUrl { get; set; }

    [StringLength(DbLength.Url)]
    [Url]
    public required string? ThumbnailUrl { get; set; }

    public required bool WithTimestamp { get; set; }
    public required bool WithServerInfoFooter { get; set; }
    public required bool IncludeAuthor { get; set; }

    [Range(0, long.MaxValue)]
    public required ulong? AuthorId { get; set; }

    public virtual UserInformation? Author { get; set; }
    public virtual ICollection<Field> Fields { get; set; } = [];
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
    public DateTime? UpdateDateUtc { get; set; }

    public DiscordEmbedBuilder CreateEmbed(Option option)
    {
        var embed = new DiscordEmbedBuilder().WithTitle(Title)
            .WithDescription(Description);

        if (!string.IsNullOrEmpty(Color)) embed.WithColor(new DiscordColor(Color));

        if (!string.IsNullOrEmpty(ImageUrl)) embed.WithImageUrl(ImageUrl);

        if (!string.IsNullOrEmpty(ThumbnailUrl)) embed.WithImageUrl(ThumbnailUrl);

        if (WithTimestamp) embed.WithCustomTimestamp();

        if (WithServerInfoFooter) embed.WithServerInfoFooter(option);

        if (IncludeAuthor && Author is not null) embed.WithUserAsAuthor(Author);

        return Fields.Aggregate(embed, (current, field) => field.AddField(current));
    }
}