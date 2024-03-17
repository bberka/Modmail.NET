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

  //TODO: Implement GreetingMessage
  public string? GreetingMessage { get; set; }

  //TODO: Implement ClosingMessage
  public string? ClosingMessage { get; set; }

  //TODO: Implement ClosingMessage
  public bool TakeFeedbackAfterClosing { get; set; }

  //TODO: Implement AllowAnonymousResponding
  public bool AllowAnonymousResponding { get; set; }

  //TODO: Implement ShowConfirmationWhenClosingTickets
  public bool ShowConfirmationWhenClosingTickets { get; set; }

  public virtual List<GuildTeam> GuildTeams { get; set; }
  public virtual List<Tag> Tags { get; set; }
  public virtual List<Ticket> Tickets { get; set; }
}