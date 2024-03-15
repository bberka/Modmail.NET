using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class TicketOption
{
  [Key]
  public Guid Id { get; set; }

  public ulong GuildId { get; set; }

  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsListenPrivateMessages { get; set; } = true;
}