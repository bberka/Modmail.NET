using System.ComponentModel.DataAnnotations;
using Modmail.NET.Database.Abstract;

namespace Modmail.NET.Database.Entities;

public class TicketType : IRegisterDateUtc, IUpdateDateUtc, IEntity, IGuidId
{
    [MaxLength(DbLength.KeyString)]
    [Required]
    public required string Key { get; set; }

    [MaxLength(DbLength.Name)]
    [Required]
    public required string Name { get; set; }

    [MaxLength(DbLength.Emoji)]
    public string? Emoji { get; set; }

    [MaxLength(DbLength.Description)]
    [Required]
    public required string Description { get; set; }

    public int Order { get; set; }
    public required Guid? EmbedId { get; set; }
    public virtual Embed? Embed { get; set; }
    public Guid Id { get; set; }
    public DateTime RegisterDateUtc { get; set; }
    public DateTime? UpdateDateUtc { get; set; }
}