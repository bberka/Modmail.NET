using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Models;

public class TicketDto
{
  public required Guid Id { get; set; }
  public required DateTime RegisterDateUtc { get; set; }
  public required DateTime? ClosedDateUtc { get; set; }
  public required DateTime LastMessageDateUtc { get; set; }
  public required bool IsForcedClosed { get; set; }
  public required bool Anonymous { get; set; }
  public UserInformation? OpenerUser { get; set; }
  public UserInformation? CloserUser { get; set; }
  public UserInformation? AssignedUser { get; set; }
  public TicketType? TicketType { get; set; }
  public string? CloseReason { get; set; }
  public int? FeedbackStar { get; set; }
  public string? FeedbackMessage { get; set; }
}