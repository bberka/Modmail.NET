using Modmail.NET.Entities;

namespace Modmail.NET.Models.Dto;

public sealed class TicketDto
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; }
  public DateTime? ClosedDateUtc { get; set; }
  public DateTime LastMessageDateUtc { get; set; }
  public bool IsForcedClosed { get; set; }
  public bool Anonymous { get; set; }
  public DiscordUserInfo OpenerUser { get; set; }
  public DiscordUserInfo? CloserUser { get; set; }
  public DiscordUserInfo? AssignedUser { get; set; }
  public TicketType? TicketType { get; set; }
  public string? CloseReason { get; set; }
}