using Modmail.NET.Entities;

namespace Modmail.NET.Models.Dto;

public sealed class TicketDto
{
  public required Guid Id { get; set; }
  public required DateTime RegisterDateUtc { get; set; }
  public required DateTime? ClosedDateUtc { get; set; }
  public required DateTime LastMessageDateUtc { get; set; }
  public required bool IsForcedClosed { get; set; }
  public required bool Anonymous { get; set; }
  public required DiscordUserInfo? OpenerUser { get; set; }
  public DiscordUserInfo? CloserUser { get; set; }
  public DiscordUserInfo? AssignedUser { get; set; }
  public TicketType? TicketType { get; set; }
  public string? CloseReason { get; set; }
}