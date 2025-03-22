using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class TicketType : IHasRegisterDate,
                                 IHasUpdateDate,
                                 IEntity
{
  public Guid Id { get; set; }

  public bool IsEnabled { get; set; } = true;

  [MaxLength(DbLength.KeyString)]
  [Required]
  public required string Key { get; set; }

  [MaxLength(DbLength.Name)]
  [Required]
  public required string Name { get; set; }

  [MaxLength(DbLength.Emoji)]
  public string Emoji { get; set; }

  [MaxLength(DbLength.Description)]
  [Required]
  public string Description { get; set; } = string.Empty;

  public int Order { get; set; }

  [MaxLength(DbLength.BotMessage)]
  public string EmbedMessageTitle { get; set; }

  [MaxLength(DbLength.BotMessage)]
  public string EmbedMessageContent { get; set; }

  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}