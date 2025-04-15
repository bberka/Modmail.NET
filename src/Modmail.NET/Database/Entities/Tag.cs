using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common.Static;
using Modmail.NET.Database.Abstract;
using Modmail.NET.Features.Tag.Helpers;

namespace Modmail.NET.Database.Entities;

[Index(nameof(Name), IsUnique = true)]
public class Tag : IRegisterDateUtc,
                   IUpdateDateUtc,
                   IEntity,
                   IGuidId
{
	[MaxLength(DbLength.TagName)]
	[Required]
	public required string Name { get; set; }

	[StringLength(DbLength.TagTitle)]
	public string? Title { get; set; } = null;

	[Required]
	[MaxLength(TagConstants.TagContentLength)]
	public required string Content { get; set; }

	public Guid Id { get; set; }
	public DateTime RegisterDateUtc { get; set; }
	public DateTime? UpdateDateUtc { get; set; }
}