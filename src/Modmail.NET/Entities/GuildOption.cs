using System.ComponentModel.DataAnnotations;

namespace Modmail.NET.Entities;

public class GuildOption
{
  [Key]
  public ulong GuildId { get; set; }

  public ulong LogChannelId { get; set; }

  public ulong CategoryId { get; set; }
  public bool IsEnabled { get; set; } = true;

  public DateTime RegisterDate { get; set; } = DateTime.Now;
  public DateTime? UpdateDate { get; set; } = DateTime.Now;

  public bool IsSensitiveLogging { get; set; } = true;

  public string GreetingMessage { get; set; }
    = "Thank you for reaching out to our team, we'll reply to you as soon as possible. Please help us speed up this process by describing your request in detail.";

  public string ClosingMessage { get; set; } = "Your ticket has been closed. If you have any further questions, feel free to open a new ticket by messaging me again.";

  //TODO: Implement TakeFeedbackAfterClosing
  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }

  public virtual List<GuildTeam> GuildTeams { get; set; }

  // public virtual List<Tag> Tags { get; set; }
  public virtual List<Ticket> Tickets { get; set; }
}