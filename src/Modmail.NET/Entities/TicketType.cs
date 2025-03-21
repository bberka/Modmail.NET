using System.ComponentModel.DataAnnotations;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class TicketType : IHasRegisterDate,
                                 IHasUpdateDate,
                                 IEntity
{
  public Guid Id { get; set; }

  public bool IsEnabled { get; set; } = true;

  [MaxLength(DbLength.KEY_STRING)]
  [Required]
  public required string Key { get; set; }

  [MaxLength(DbLength.NAME)]
  [Required]
  public required string Name { get; set; }

  [MaxLength(DbLength.EMOJI)]
  public string Emoji { get; set; }

  [MaxLength(DbLength.DESCRIPTION)]
  [Required]
  public string Description { get; set; } = string.Empty;

  public int Order { get; set; }

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string EmbedMessageTitle { get; set; }

  [MaxLength(DbLength.BOT_MESSAGE)]
  public string EmbedMessageContent { get; set; }

  public DateTime RegisterDateUtc { get; set; }
  public DateTime? UpdateDateUtc { get; set; }
}