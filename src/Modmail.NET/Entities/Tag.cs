using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class Tag
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDate { get; set; }
  public DateTime? UpdateDate { get; set; }

  public ulong GuildId { get; set; }

  public string Key { get; set; }

  public string MessageContent { get; set; }

  public bool UseEmbed { get; set; } = false;
}