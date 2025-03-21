using DSharpPlus.Entities;
using MediatR;

namespace Modmail.NET.Features.Ticket;

public sealed record ProcessCloseTicketCommand(
  Guid TicketId,
  ulong CloserUserId = 0,
  string CloseReason = null,
  DiscordChannel ModChatChannel = null,
  bool DontSendFeedbackMessage = false) : IRequest;

public sealed record ProcessChangePriorityCommand(
  Guid TicketId,
  ulong ModUserId,
  TicketPriority NewPriority,
  DiscordChannel TicketChannel = null) : IRequest;

public sealed record ProcessUserSentMessageCommand(
  Guid TicketId,
  DiscordMessage Message,
  DiscordChannel PrivateChannel = null) : IRequest;

public sealed record ProcessCreateNewTicketCommand(
  DiscordUser User,
  DiscordChannel PrivateChannel,
  DiscordMessage Message) : IRequest;

public sealed record ProcessModSendMessageCommand(
  Guid TicketId,
  DiscordUser ModUser,
  DiscordMessage Message,
  DiscordChannel Channel,
  DiscordGuild Guild
) : IRequest;

public sealed record ProcessAddFeedbackCommand(
  Guid TicketId,
  int StarCount,
  string TextInput,
  DiscordMessage FeedbackMessage
) : IRequest;

public sealed record ProcessAddNoteCommand(
  Guid TicketId,
  ulong UserId,
  string Note
) : IRequest;

public sealed record ProcessToggleAnonymousCommand(
  Guid TicketId,
  DiscordChannel TicketChannel = null
) : IRequest;

public sealed record ProcessChangeTicketTypeCommand(
  Guid TicketId,
  string Type,
  DiscordChannel TicketChannel = null,
  DiscordChannel PrivateChannel = null,
  DiscordMessage PrivateMessageWithComponent = null,
  ulong UserId = 0
) : IRequest;