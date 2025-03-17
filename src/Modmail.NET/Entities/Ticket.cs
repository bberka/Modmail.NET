using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Manager;
using Modmail.NET.Utils;

namespace Modmail.NET.Entities;

public sealed class Ticket
{
  public Guid Id { get; set; }
  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
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
  public string? CloseReason { get; set; }

  public bool IsForcedClosed { get; set; }

  public int? FeedbackStar { get; set; }

  [MaxLength(DbLength.FEEDBACK_MESSAGE)]
  public string? FeedbackMessage { get; set; }

  public bool Anonymous { get; set; }

  [ForeignKey(nameof(TicketType))]
  public Guid? TicketTypeId { get; set; }

  //FK

  public DiscordUserInfo? OpenerUser { get; set; }
  public DiscordUserInfo? CloserUser { get; set; }
  public DiscordUserInfo? AssignedUser { get; set; }
  public TicketType? TicketType { get; set; }
  public List<TicketMessage>? Messages { get; set; }
  public List<TicketNote>? TicketNotes { get; set; }

  public async Task AddAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.Tickets.AddAsync(this);
    await dbContext.SaveChangesAsync();
  }

  public async Task UpdateAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.Tickets.Update(this);
    await dbContext.SaveChangesAsync();
  }


  public static async Task<Ticket> GetActiveTicketAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets
                                .FirstOrDefaultAsync(x => x.OpenerUserId == userId && !x.ClosedDateUtc.HasValue);
    if (ticket is null) throw new NotFoundException(LangKeys.TICKET);
    return ticket;
  }

  public static async Task<List<Ticket>> GetAllTickets(int page = 1, int pageSize = 25) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.Tickets
                          .ToListAsync();
  }

  public static async Task<Ticket?> GetActiveTicketNullableAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets
                                .FirstOrDefaultAsync(x => x.OpenerUserId == userId && !x.ClosedDateUtc.HasValue);
    return ticket;
  }

  public static async Task<Ticket?> GetActiveTicketNullableAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FindAsync(id);
    return ticket;
  }

  public static async Task<Ticket> GetActiveTicketAsync(Guid ticketId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId && !x.ClosedDateUtc.HasValue);
    if (ticket is null) throw new NotFoundException(LangKeys.TICKET);
    return ticket;
  }

  public static async Task<Ticket> GetAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
    if (ticket is null) throw new NotFoundException(LangKeys.TICKET);
    return ticket;
  }

  public static async Task<Ticket?> GetNullableAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FindAsync(id);
    return ticket;
  }

  public async Task ProcessCloseTicketAsync(ulong closerUserId = 0,
                                            string? closeReason = null,
                                            DiscordChannel? modChatChannel = null,
                                            bool dontSendFeedbackMessage = false) {
    ArgumentNullException.ThrowIfNull(OpenerUser);

    if (closerUserId == 0) closerUserId = ModmailBot.This.Client.CurrentUser.Id;
    if (string.IsNullOrEmpty(closeReason)) closeReason = LangData.This.GetTranslation(LangKeys.NO_REASON_PROVIDED);
    if (ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();
    CloserUser = await DiscordUserInfo.GetAsync(closerUserId);
    ArgumentNullException.ThrowIfNull(CloserUser);


    var guildOption = await GuildOption.GetAsync();

    ClosedDateUtc = DateTime.UtcNow;
    CloseReason = closeReason;
    CloserUserId = closerUserId;

    var closerUser = await DiscordUserInfo.GetAsync(closerUserId);
    CloserUser = closerUser;

    await UpdateAsync();

    _ = Task.Run(async () => {
      modChatChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      //Don't await this task
      await modChatChannel.DeleteAsync(LangData.This.GetTranslation(LangKeys.TICKET_CLOSED));
      var pmChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
      if (pmChannel != null) {
        await pmChannel.SendMessageAsync(UserResponses.YourTicketHasBeenClosed(this, guildOption));
        if (guildOption.TakeFeedbackAfterClosing && !dontSendFeedbackMessage) await pmChannel.SendMessageAsync(UserResponses.GiveFeedbackMessage(this, guildOption));
      }


      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketClosed(this));
      }
    });
  }

  public async Task ProcessChangePriorityAsync(ulong modUserId,
                                               TicketPriority newPriority,
                                               DiscordChannel? ticketChannel = null) {
    ArgumentNullException.ThrowIfNull(OpenerUser);
    if (modUserId == 0) throw new InvalidUserIdException();
    if (ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();

    var oldPriority = Priority;
    Priority = newPriority;

    await UpdateAsync();

    _ = Task.Run(async () => {
      //Don't await this task
      // var modUser = await ModmailBot.This.Client.GetUserAsync(modUserId);
      // if (modUser is null) throw new InvalidOperationException("ModUser is null");

      var guildOption = await GuildOption.GetAsync();

      var modUser = await DiscordUserInfo.GetAsync(modUserId);
      if (modUser is null) throw new InvalidOperationException("ModUser is null");

      var privateChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
      if (privateChannel is not null) await privateChannel.SendMessageAsync(UserResponses.TicketPriorityChanged(guildOption, modUser, this, oldPriority, newPriority));

      //TODO: Handle private messageContent privateChannel not found
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketPriorityChanged(modUser, this, oldPriority, newPriority));
      }

      ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (ticketChannel is not null) {
        var newChName = newPriority switch {
          TicketPriority.Normal => Const.NORMAL_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUser.Username.Trim()),
          TicketPriority.High => Const.HIGH_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUser.Username.Trim()),
          TicketPriority.Low => Const.LOW_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUser.Username.Trim()),
          _ => ""
        };

        await ticketChannel.ModifyAsync(x => { x.Name = newChName; });


        await ticketChannel.SendMessageAsync(TicketResponses.TicketPriorityChanged(modUser, this, oldPriority, newPriority));
      }
      //TODO: Handle ticket privateChannel not found
    });
  }


  public async Task ProcessUserSentMessageAsync(DiscordMessage message,
                                                DiscordChannel? privateChannel = null) {
    ArgumentNullException.ThrowIfNull(OpenerUser);
    ArgumentNullException.ThrowIfNull(message);
    ArgumentNullException.ThrowIfNull(message);
    await Task.Delay(50); //wait for privateChannel creation process to finish

    var guildOption = await GuildOption.GetAsync();


    LastMessageDateUtc = DateTime.UtcNow;
    await UpdateAsync();

    var ticketMessage = TicketMessage.MapFrom(Id, message);
    await ticketMessage.AddAsync();


    _ = Task.Run(async () => {
      var user = message.Author;

      privateChannel ??= await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
      if (privateChannel is null) throw new NotFoundWithException(LangKeys.CHANNEL, PrivateMessageChannelId);

      var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (mailChannel is not null) {
        var permissions = await GuildTeamMember.GetPermissionInfoAsync();
        await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(message, permissions));
      }

      //TODO: Handle mail privateChannel not found
      if (guildOption.IsEnableDiscordChannelLogging)
        if (guildOption.IsSensitiveLogging) {
          await privateChannel.SendMessageAsync(UserResponses.MessageSent(message));

          //Don't await this task
          var logChannel = await ModmailBot.This.GetLogChannelAsync();
          await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(message, Id));
        }
    });
  }

  public static async Task ProcessCreateNewTicketAsync(DiscordUser user, DiscordChannel privateChannel, DiscordMessage message) {
    var guildOption = await GuildOption.GetAsync();
    if (guildOption is null) throw new ServerIsNotSetupException();

    var guild = await ModmailBot.This.GetMainGuildAsync();
    //make new privateChannel
    var channelName = string.Format(Const.TICKET_NAME_TEMPLATE, user.Username.Trim());
    var category = await ModmailBot.This.Client.GetChannelAsync(guildOption.CategoryId);

    var ticketId = Guid.NewGuid();
    var messageId = message.Id;

    var permissions = await GuildTeamMember.GetPermissionInfoAsync();
    var members = await guild.GetAllMembersAsync();
    var roles = guild.Roles;

    var (modMemberListForOverwrites, modRoleListForOverwrites) = UtilPermission.ParsePermissionInfo(permissions, members, roles);
    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, modMemberListForOverwrites, modRoleListForOverwrites);
    var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

    var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
    if (member is null) return;


    var newTicketMessageBuilder = TicketResponses.NewTicket(member, ticketId, permissions);


    var ticketMessage = TicketMessage.MapFrom(ticketId, message);

    var ticket = new Ticket {
      OpenerUserId = user.Id,
      ModMessageChannelId = mailChannel.Id,
      RegisterDateUtc = DateTime.UtcNow,
      PrivateMessageChannelId = privateChannel.Id,
      InitialMessageId = message.Id,
      Priority = TicketPriority.Normal,
      LastMessageDateUtc = DateTime.UtcNow,
      Id = ticketId,
      Anonymous = guildOption.AlwaysAnonymous,
      IsForcedClosed = false,
      CloseReason = null,
      FeedbackMessage = null,
      FeedbackStar = null,
      CloserUserId = null,
      ClosedDateUtc = null,
      TicketTypeId = null,
      Messages = new List<TicketMessage> {
        ticketMessage
      },
      BotTicketCreatedMessageInDmId = 0
    };


    await ticket.AddAsync();

    var ticketTypes = await TicketType.GetAllActiveAsync();

    var ticketCreatedMessage = await privateChannel.SendMessageAsync(UserResponses.YouHaveCreatedNewTicket(guild,
                                                                                                           guildOption,
                                                                                                           ticketTypes,
                                                                                                           ticketId));

    TicketTypeSelectionTimeoutTimer.This.AddMessage(ticketCreatedMessage);


    var dmTicketCreatedMessage = await privateChannel.SendMessageAsync(UserResponses.MessageSent(message));

    ticket.BotTicketCreatedMessageInDmId = dmTicketCreatedMessage.Id;
    await ticket.UpdateAsync();


    _ = Task.Run(async () => {
      await mailChannel.SendMessageAsync(newTicketMessageBuilder);
      await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(message));


      if (guildOption.IsEnableDiscordChannelLogging) {
        var newTicketCreatedLog = LogResponses.NewTicketCreated(message, mailChannel, ticket.Id);
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(newTicketCreatedLog);
        if (guildOption.IsSensitiveLogging) await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(message, ticket.Id));
      }
    });
  }

  public async Task ProcessModSendMessageAsync(DiscordUser modUser,
                                               DiscordMessage message,
                                               DiscordChannel channel,
                                               DiscordGuild guild) {
    ArgumentNullException.ThrowIfNull(modUser);
    ArgumentNullException.ThrowIfNull(message);
    ArgumentNullException.ThrowIfNull(channel);
    ArgumentNullException.ThrowIfNull(guild);


    LastMessageDateUtc = DateTime.UtcNow;
    await UpdateAsync();

    var guildOption = await GuildOption.GetAsync();
    _ = Task.Run(async () => {
      var privateChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
      if (privateChannel is not null) {
        var embed = UserResponses.MessageReceived(message, Anonymous);
        await privateChannel.SendMessageAsync(embed);
      }

      var ticketChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (ticketChannel is not null) {
        var embed2 = TicketResponses.MessageSent(message, Anonymous);
        await ticketChannel.SendMessageAsync(embed2);
        await message.DeleteAsync();
      }

      if (guildOption.IsEnableDiscordChannelLogging)
        if (guildOption.IsSensitiveLogging) {
          //Don't await this task
          var ticketMessage = TicketMessage.MapFrom(Id, message);
          await ticketMessage.AddAsync();


          var logChannel = await ModmailBot.This.GetLogChannelAsync();
          var embed3 = LogResponses.MessageSentByMod(message,
                                                     Id,
                                                     Anonymous);
          await logChannel.SendMessageAsync(embed3);
        }
    });
  }

  public async Task ProcessAddFeedbackAsync(int starCount, string textInput, DiscordMessage feedbackMessage) {
    if (OpenerUser is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (!ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket must be closed");

    if (starCount < 1 || starCount > 5) throw new InvalidOperationException("Star count must be between 1 and 5");

    var guildOption = await GuildOption.GetAsync();
    if (!guildOption.TakeFeedbackAfterClosing) throw new InvalidOperationException("Feedback is not enabled for this guild: " + guildOption.GuildId);

    if (string.IsNullOrEmpty(textInput)) throw new InvalidOperationException("Feedback messageContent is empty");

    if (feedbackMessage is null) throw new InvalidOperationException("Feedback messageContent is null");

    FeedbackStar = starCount;
    FeedbackMessage = textInput;
    await UpdateAsync();

    // var mainGuild = await ModmailBot.This.GetMainGuildAsync();

    _ = Task.Run(async () => {
      //Don't await this task
      await feedbackMessage.ModifyAsync(x => { x.AddEmbed(UserResponses.FeedbackReceivedUpdateMessage(this)); });

      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.FeedbackReceived(this));
      }
    });
  }

  public async Task ProcessAddNoteAsync(ulong userId, string note) {
    if (OpenerUser is null) throw new InvalidOperationException("OpenerUserInfo is null");

    var guildOption = await GuildOption.GetAsync();
    var noteEntity = new TicketNote {
      TicketId = Id,
      Content = note,
      DiscordUserId = userId,
      RegisterDateUtc = DateTime.UtcNow
    };
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketNotes.AddAsync(noteEntity);
    await dbContext.SaveChangesAsync();

    _ = Task.Run(async () => {
      //Don't await this task

      var user = await DiscordUserInfo.GetAsync(userId);

      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.NoteAdded(this, noteEntity, user));
      }


      var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (mailChannel is not null) await mailChannel.SendMessageAsync(TicketResponses.NoteAdded(noteEntity, user));
    });
  }

  public async Task ProcessToggleAnonymousAsync(DiscordChannel? ticketChannel = null) {
    if (OpenerUser is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket is closed");

    Anonymous = !Anonymous;
    await UpdateAsync();

    _ = Task.Run(async () => {
      //Don't await this task

      ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketResponses.AnonymousToggled(this));

      //TODO: Handle mail channel not found
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.AnonymousToggled(this));
      }
    });
  }

  public async Task ProcessChangeTicketTypeAsync(string type,
                                                 DiscordChannel? ticketChannel = null,
                                                 DiscordChannel? privateChannel = null,
                                                 DiscordMessage? privateMessageWithComponent = null,
                                                 ulong userId = 0) {
    if (OpenerUser is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (userId == 0) userId = ModmailBot.This.Client.CurrentUser.Id;
    if (ClosedDateUtc.HasValue)
      //TODO: maybe add removal of embeds for the message to keep getting called if ticket is closed
      throw new TicketAlreadyClosedException();

    var ticketType = await TicketType.GetNullableAsync(type);
    if (ticketType is null)
      TicketTypeId = null;
    else
      TicketTypeId = ticketType.Id;

    await UpdateAsync();

    await Task.Run(async () => {
      //Don't await this task
      var userInfo = await DiscordUserInfo.GetAsync(userId);
      ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
      if (ticketChannel is not null) await ticketChannel.SendMessageAsync(TicketResponses.TicketTypeChanged(userInfo, ticketType));

      //TODO: Handle mail channel not found
      var privateChannelId = PrivateMessageChannelId;
      privateChannel ??= await ModmailBot.This.Client.GetChannelAsync(privateChannelId);
      if (privateChannel is not null)
        // await privateChannel.SendMessageAsync(UserResponses.TicketTypeChanged(ticketType));
        if (BotTicketCreatedMessageInDmId != 0) {
          privateMessageWithComponent ??= await privateChannel.GetMessageAsync(BotTicketCreatedMessageInDmId);
          if (privateMessageWithComponent is not null) {
            var newEmbed = new DiscordEmbedBuilder(privateMessageWithComponent.Embeds[0]);
            if (ticketType is not null) {
              var emoji = DiscordEmoji.FromUnicode(ModmailBot.This.Client, ticketType.Emoji);
              var typeName = ticketType.Name;
              var str = $"{emoji} {typeName}";
              newEmbed.AddField(LangKeys.TICKET_TYPE.GetTranslation(), str);
            }

            await privateMessageWithComponent.ModifyAsync(x => {
              x.ClearComponents();
              x.AddEmbed(newEmbed);
            });

            TicketTypeSelectionTimeoutTimer.This.RemoveMessage(privateMessageWithComponent.Id);
          }
        }

      //TODO: Handle private channel not found
      var guildOption = await GuildOption.GetAsync();
      if (guildOption.IsEnableDiscordChannelLogging) {
        var logChannel = await ModmailBot.This.GetLogChannelAsync();
        await logChannel.SendMessageAsync(LogResponses.TicketTypeChanged(this, ticketType));
      }
    });
  }

  public static async Task<List<Ticket>> GetActiveTicketsAsync() {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var tickets = await dbContext.Tickets
                                 .Where(x => !x.ClosedDateUtc.HasValue)
                                 .ToListAsync();
    return tickets;
  }


  public static async Task<List<Ticket>> GetTimeoutTicketsAsync(long timeoutHours) {
    if (timeoutHours < Const.TICKET_TIMEOUT_MIN_ALLOWED_HOURS) timeoutHours = Const.DEFAULT_TICKET_TIMEOUT_HOURS;

    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var timeoutDate = DateTime.UtcNow.AddHours(-timeoutHours);
    var tickets = await dbContext.Tickets
                                 .Where(x => !x.ClosedDateUtc.HasValue && x.LastMessageDateUtc < timeoutDate)
                                 .ToListAsync();
    return tickets;
  }

  public static async Task<List<Ticket>> GetAllByTypeAsync(TicketType ticketType) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var tickets = await dbContext.Tickets
                                 .Where(x => x.TicketTypeId == ticketType.Id)
                                 .ToListAsync();
    return tickets;
  }

  public static async Task<bool> AnyActiveTicketsByTypeAsync(TicketType ticketType) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    return await dbContext.Tickets
                          .AnyAsync(x => x.TicketTypeId == ticketType.Id && !x.ClosedDateUtc.HasValue);
  }

  public static async Task UpdateRangeAsync(List<Ticket> allTicketsByType) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    dbContext.Tickets.UpdateRange(allTicketsByType);
    await dbContext.SaveChangesAsync();
  }
}