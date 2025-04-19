using System.ComponentModel.DataAnnotations;
using DSharpPlus.Entities;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class Field : IEntity,
                     IGuidId,
                     IRegisterDateUtc,
                     IUpdateDateUtc
{
	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public DateTime? UpdateDateUtc { get; set; }

	[StringLength(DbLength.Title)]
	public required string Title { get; set; }

	[StringLength(DbLength.Message)]
	public required string Content { get; set; }

	public required bool Inline { get; set; } = false;

	public DiscordEmbedBuilder AddField(DiscordEmbedBuilder builder) {
		return builder.AddField(Title, Content, Inline);
	}
}