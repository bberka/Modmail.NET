﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Modmail.NET.Abstract;

namespace Modmail.NET.Entities;

public sealed class Ticket : IHasRegisterDate,
                             IEntity
{
  public Guid Id { get; set; }
  public DateTime LastMessageDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? ClosedDateUtc { get; set; }
  public ulong OpenerUserId { get; set; } //FK
  public ulong? CloserUserId { get; set; } //FK
  public ulong? AssignedUserId { get; set; } //FK
  public ulong PrivateMessageChannelId { get; set; }
  public ulong ModMessageChannelId { get; set; }
  public ulong InitialMessageId { get; set; }
  public ulong BotTicketCreatedMessageInDmId { get; set; }
  public TicketPriority Priority { get; set; }

  [MaxLength(DbLength.REASON)]
  public string CloseReason { get; set; }

  public bool IsForcedClosed { get; set; }
  public int? FeedbackStar { get; set; }

  [MaxLength(DbLength.FEEDBACK_MESSAGE)]
  public string FeedbackMessage { get; set; }

  public bool Anonymous { get; set; }

  [ForeignKey(nameof(TicketType))]
  public Guid? TicketTypeId { get; set; }

  //FK

  public DiscordUserInfo OpenerUser { get; set; }
  public DiscordUserInfo CloserUser { get; set; }
  public DiscordUserInfo AssignedUser { get; set; }
  public TicketType TicketType { get; set; }
  public List<TicketMessage> Messages { get; set; } = [];
  public List<TicketNote> TicketNotes { get; set; } = [];
  public DateTime RegisterDateUtc { get; set; }
}