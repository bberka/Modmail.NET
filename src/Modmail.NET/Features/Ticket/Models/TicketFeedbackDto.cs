using Modmail.NET.Database.Entities;

namespace Modmail.NET.Features.Ticket.Models;

public class TicketFeedbackDto
{
  public required Guid Id { get; set; }
  public required DateTime ClosedDateUtc { get; set; }
  public required DiscordUserInfo OpenerUser { get; set; }
  public int? FeedbackStar { get; set; }
  public string FeedbackMessage { get; set; }
}