using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Manager;
using Modmail.NET.Static;
using Modmail.NET.Utils;
using Serilog;

namespace Modmail.NET.Entities;

public class Ticket
{
  [Key]
  public Guid Id { get; set; }

  public DateTime RegisterDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime LastMessageDateUtc { get; set; } = DateTime.UtcNow;
  public DateTime? ClosedDateUtc { get; set; }

  [ForeignKey(nameof(OpenerUserInfo))]
  public ulong OpenerUserId { get; set; } //FK

  [ForeignKey(nameof(CloserUserInfo))]
  public ulong? CloserUserId { get; set; } //FK

  public ulong PrivateMessageChannelId { get; set; }
  public ulong ModMessageChannelId { get; set; }
  public ulong InitialMessageId { get; set; }
  public ulong BotTicketCreatedMessageInDmId { get; set; }
  public TicketPriority Priority { get; set; }
  public string? CloseReason { get; set; }
  public bool IsForcedClosed { get; set; } = false;

  [ForeignKey(nameof(GuildOption))]
  public ulong GuildOptionId { get; set; }

  public int? FeedbackStar { get; set; }
  public string? FeedbackMessage { get; set; }

  public bool Anonymous { get; set; }

  [ForeignKey(nameof(TicketType))]
  public Guid? TicketTypeId { get; set; }

  //FK

  public virtual DiscordUserInfo OpenerUserInfo { get; set; }
  public virtual DiscordUserInfo? CloserUserInfo { get; set; }

  public virtual GuildOption GuildOption { get; set; }
  public virtual TicketType? TicketType { get; set; }

  public virtual List<TicketMessage> TicketMessages { get; set; }

  public virtual List<TicketNote> TicketNotes { get; set; }

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
    if (ticket is null) throw new TicketNotFoundException();
    return ticket;
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
    if (ticket is null) throw new TicketNotFoundException();
    return ticket;
  }

  public static async Task<Ticket> GetAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
    if (ticket is null) throw new TicketNotFoundException();
    return ticket;
  }

  public static async Task<Ticket?> GetNullableAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FindAsync(id);
    return ticket;
  }

  public async Task ProcessCloseTicketAsync(ulong closerUserId,
                                            string? closeReason = null,
                                            DiscordChannel? modChatChannel = null,
                                            bool dontSendFeedbackMessage = false) {
    ArgumentNullException.ThrowIfNull(GuildOption);
    ArgumentNullException.ThrowIfNull(OpenerUserInfo);
    if (closerUserId == 0) throw new InvalidUserIdException();
    if (string.IsNullOrEmpty(closeReason)) closeReason = Texts.NO_REASON_PROVIDED;
    if (ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();
    CloserUserInfo = await DiscordUserInfo.GetAsync(closerUserId);
    ArgumentNullException.ThrowIfNull(CloserUserInfo);

    modChatChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);


    ClosedDateUtc = DateTime.UtcNow;
    CloseReason = closeReason;
    CloserUserId = closerUserId;

    var closerUser = await DiscordUserInfo.GetAsync(closerUserId);
    CloserUserInfo = closerUser;

    await UpdateAsync();

    await modChatChannel.DeleteAsync(Texts.TICKET_CLOSED);

    var pmChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (pmChannel != null) {
      await pmChannel.SendMessageAsync(UserResponses.YourTicketHasBeenClosed(this));
      if (GuildOption.TakeFeedbackAfterClosing && !dontSendFeedbackMessage) {
        await pmChannel.SendMessageAsync(UserResponses.GiveFeedbackMessage(this));
      }
    }
    else {
      Log.Warning("Private messageContent channel not found {TicketId} {ChannelId}", Id, PrivateMessageChannelId);
    }

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    await logChannel.SendMessageAsync(LogResponses.TicketClosed(this));
  }

  public async Task ProcessChangePriority(ulong modUserId,
                                          TicketPriority newPriority,
                                          DiscordChannel? ticketChannel = null) {
    ArgumentNullException.ThrowIfNull(GuildOption);
    ArgumentNullException.ThrowIfNull(OpenerUserInfo);
    if (modUserId == 0) throw new InvalidUserIdException();
    if (ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();

    var modUser = await DiscordUserInfo.GetAsync(modUserId);
    if (modUser is null) throw new InvalidOperationException("ModUser is null");

    var oldPriority = Priority;
    Priority = newPriority;

    await this.UpdateAsync();

    // var modUser = await ModmailBot.This.Client.GetUserAsync(modUserId);
    // if (modUser is null) throw new InvalidOperationException("ModUser is null");

    var privateChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (privateChannel is not null) {
      await privateChannel.SendMessageAsync(UserResponses.TicketPriorityChanged(modUser, this, oldPriority, newPriority));
    }
    else {
      //TODO: Handle private messageContent privateChannel not found
    }


    var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);
    if (logChannel is not null) {
      await logChannel.SendMessageAsync(LogResponses.TicketPriorityChanged(modUser, this, oldPriority, newPriority));
    }
    else {
      //TODO: Handle log privateChannel not found
    }

    ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      var newChName = "";
      switch (newPriority) {
        case TicketPriority.Normal:
          newChName = Const.NORMAL_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUserInfo.Username.Trim());
          break;
        case TicketPriority.High:
          newChName = Const.HIGH_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUserInfo.Username.Trim());
          break;
        case TicketPriority.Low:
          newChName = Const.LOW_PRIORITY_EMOJI + string.Format(Const.TICKET_NAME_TEMPLATE, OpenerUserInfo.Username.Trim());
          break;
      }

      await ticketChannel.ModifyAsync(x => { x.Name = newChName; });


      await ticketChannel.SendMessageAsync(TicketResponses.TicketPriorityChanged(modUser, this, oldPriority, newPriority));
    }
    else {
      //TODO: Handle ticket privateChannel not found
    }
  }


  public async Task ProcessUserSentMessageAsync(DiscordMessage message,
                                                DiscordChannel? privateChannel = null) {
    ArgumentNullException.ThrowIfNull(GuildOption);
    ArgumentNullException.ThrowIfNull(OpenerUserInfo);
    ArgumentNullException.ThrowIfNull(message);
    ArgumentNullException.ThrowIfNull(message);
    await Task.Delay(50); //wait for privateChannel creation process to finish

    var user = message.Author;

    privateChannel ??= await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (privateChannel is null) throw new ChannelNotFoundException(PrivateMessageChannelId);

    LastMessageDateUtc = DateTime.UtcNow;
    await this.UpdateAsync();

    var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (mailChannel is not null) {
      await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(message));
    }
    else {
      //TODO: Handle mail privateChannel not found
    }

    var ticketMessage = TicketMessage.MapFrom(Id, message);
    await ticketMessage.AddAsync();

    await privateChannel.SendMessageAsync(UserResponses.MessageSent(message));

    if (GuildOption.IsSensitiveLogging) {
      var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);

      if (logChannel is not null) {
        await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(message, Id));
      }
      else {
        //TODO: Handle log privateChannel not found
      }
    }
  }

  public static async Task ProcessCreateNewTicketAsync(DiscordUser user, DiscordChannel privateChannel, DiscordMessage message) {
    var guildOption = await Entities.GuildOption.GetAsync();
    if (guildOption is null) throw new ServerIsNotSetupException();

    var guild = await ModmailBot.This.GetMainGuildAsync();
    //make new privateChannel
    var channelName = string.Format(Const.TICKET_NAME_TEMPLATE, user.Username.Trim());
    var category = await ModmailBot.This.Client.GetChannelAsync(guildOption.CategoryId);

    var ticketId = Guid.NewGuid();
    var messageId = message.Id;

    var permissions = await GuildTeamMember.GetPermissionInfoAsync(guildOption.CategoryId);
    var members = await guild.GetAllMembersAsync();
    var roles = guild.Roles;

    var (modMemberListForOverwrites, modRoleListForOverwrites) = UtilPermission.ParsePermissionInfo(permissions, members, roles);
    var permissionOverwrites = UtilPermission.GetTicketPermissionOverwrites(guild, modMemberListForOverwrites, modRoleListForOverwrites);
    var mailChannel = await guild.CreateTextChannelAsync(channelName, category, UtilChannelTopic.BuildChannelTopic(ticketId), permissionOverwrites);

    var member = await ModmailBot.This.GetMemberFromAnyGuildAsync(user.Id);
    if (member is null) {
      return;
    }


    var newTicketMessageBuilder = TicketResponses.NewTicket(member, ticketId, modRoleListForOverwrites, modMemberListForOverwrites);
    await mailChannel.SendMessageAsync(newTicketMessageBuilder);
    await mailChannel.SendMessageAsync(TicketResponses.MessageReceived(message));

    var ticketMessage = TicketMessage.MapFrom(ticketId, message);

    var ticket = new Ticket {
      OpenerUserId = user.Id,
      ModMessageChannelId = mailChannel.Id,
      RegisterDateUtc = DateTime.UtcNow,
      PrivateMessageChannelId = privateChannel.Id,
      InitialMessageId = message.Id,
      Priority = TicketPriority.Normal,
      LastMessageDateUtc = DateTime.UtcNow,
      GuildOptionId = guildOption.GuildId,
      Id = ticketId,
      Anonymous = false,
      IsForcedClosed = false,
      CloseReason = null,
      FeedbackMessage = null,
      FeedbackStar = null,
      CloserUserId = null,
      ClosedDateUtc = null,
      TicketTypeId = null,
      TicketMessages = new List<TicketMessage>() {
        ticketMessage
      },
      BotTicketCreatedMessageInDmId = 0
    };


    await ticket.AddAsync();

    var ticketTypes = await TicketType.GetAllAsync();

    var ticketCreatedMessage = await privateChannel.SendMessageAsync(UserResponses.YouHaveCreatedNewTicket(guild,
                                                                                                           guildOption,
                                                                                                           ticketTypes,
                                                                                                           ticketId));

    TicketTypeSelectionTimeoutMgr.This.AddMessage(ticketCreatedMessage);
    var dmTicketCreatedMessage = await privateChannel.SendMessageAsync(UserResponses.MessageSent(message));

    ticket.BotTicketCreatedMessageInDmId = dmTicketCreatedMessage.Id;
    await ticket.UpdateAsync();

    var newTicketCreatedLog = LogResponses.NewTicketCreated(message, mailChannel, ticket.Id);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(guildOption.LogChannelId);

    if (logChannel is not null) {
      await logChannel.SendMessageAsync(newTicketCreatedLog);
      if (guildOption.IsSensitiveLogging) {
        await logChannel.SendMessageAsync(LogResponses.MessageSentByUser(message, ticket.Id));
      }
    }
  }

  public async Task ProcessModSendMessageAsync(DiscordUser modUser,
                                               DiscordMessage message,
                                               DiscordChannel channel,
                                               DiscordGuild guild) {
    ArgumentNullException.ThrowIfNull(modUser);
    ArgumentNullException.ThrowIfNull(message);
    ArgumentNullException.ThrowIfNull(channel);
    ArgumentNullException.ThrowIfNull(guild);

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


    LastMessageDateUtc = DateTime.UtcNow;
    await UpdateAsync();


    if (GuildOption.IsSensitiveLogging) {
      var ticketMessage = TicketMessage.MapFrom(Id, message);
      await ticketMessage.AddAsync();


      var logChannelId = GuildOption.LogChannelId;
      var logChannel = guild.GetChannel(logChannelId);
      var embed3 = LogResponses.MessageSentByMod(message,
                                                 Id,
                                                 Anonymous);
      await logChannel.SendMessageAsync(embed3);
    }
  }

  public async Task ProcessAddFeedbackAsync(int starCount, string textInput, DiscordMessage feedbackMessage) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (!ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket must be closed");

    if (starCount < 1 || starCount > 5) {
      throw new InvalidOperationException("Star count must be between 1 and 5");
    }

    if (!GuildOption.TakeFeedbackAfterClosing) {
      throw new InvalidOperationException("Feedback is not enabled for this guild: " + GuildOptionId);
    }

    if (string.IsNullOrEmpty(textInput)) {
      throw new InvalidOperationException("Feedback messageContent is empty");
    }

    if (feedbackMessage is null) {
      throw new InvalidOperationException("Feedback messageContent is null");
    }

    FeedbackStar = starCount;
    FeedbackMessage = textInput;
    await UpdateAsync();

    var mainGuild = await ModmailBot.This.GetMainGuildAsync();


    await feedbackMessage.ModifyAsync(x => { x.AddEmbed(UserResponses.FeedbackReceivedUpdateMessage(this)); });

    var logChannel = mainGuild.GetChannel(GuildOption.LogChannelId);
    if (logChannel is not null) {
      await logChannel.SendMessageAsync(LogResponses.FeedbackReceived(this));
    }
  }

  public async Task ProcessAddNoteAsync(ulong userId, string note) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    var noteEntity = new TicketNote {
      TicketId = Id,
      Content = note,
      DiscordUserId = userId,
      RegisterDateUtc = DateTime.UtcNow,
    };
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    await dbContext.TicketNotes.AddAsync(noteEntity);
    await dbContext.SaveChangesAsync();

    var user = await DiscordUserInfo.GetAsync(userId);

    var logChannelId = await GuildOption.GetLogChannelIdAsync(GuildOption.GuildId);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(logChannelId);
    if (logChannel is not null) {
      await logChannel.SendMessageAsync(LogResponses.NoteAdded(this, noteEntity, user));
    }
    else {
      //TODO: Handle log privateChannel not found
    }


    var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (mailChannel is not null) {
      await mailChannel.SendMessageAsync(TicketResponses.NoteAdded(noteEntity, user));
    }
    else {
      //TODO: Handle mail channel not found
    }
  }

  public async Task ProcessToggleAnonymousAsync(DiscordChannel? ticketChannel = null) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket is closed");

    Anonymous = !Anonymous;
    await UpdateAsync();
    var logChannelId = await GuildOption.GetLogChannelIdAsync(GuildOption.GuildId);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(logChannelId);
    if (logChannel is not null) {
      await logChannel.SendMessageAsync(LogResponses.AnonymousToggled(this));
    }
    else {
      //TODO: Handle log channel not found
    }

    ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      await ticketChannel.SendMessageAsync(TicketResponses.AnonymousToggled(this));
    }
    else {
      //TODO: Handle mail channel not found
    }
  }

  public async Task ProcessChangeTicketTypeAsync(ulong userId,
                                                 string type,
                                                 DiscordChannel? ticketChannel = null,
                                                 DiscordChannel? privateChannel = null,
                                                 DiscordMessage? privateMessageWithComponent = null) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (userId == 0) throw new InvalidOperationException("UserId is 0");
    if (ClosedDateUtc.HasValue) {
      //TODO: maybe add removal of embeds for the message to keep getting called if ticket is closed
      throw new TicketAlreadyClosedException();
    }

    var ticketType = await Entities.TicketType.GetAsync(type);
    if (ticketType is null) throw new InvalidOperationException("TicketType is null");

    TicketTypeId = ticketType.Id;
    await UpdateAsync();

    var logChannelId = await GuildOption.GetLogChannelIdAsync(GuildOption.GuildId);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(logChannelId);
    if (logChannel is not null) {
      await logChannel.SendMessageAsync(LogResponses.TicketTypeChanged(this, ticketType));
    }
    else {
      //TODO: Handle log channel not found
    }

    var userInfo = await DiscordUserInfo.GetAsync(userId);
    ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      await ticketChannel.SendMessageAsync(TicketResponses.TicketTypeChanged(userInfo, ticketType));
    }
    else {
      //TODO: Handle mail channel not found
    }

    var privateChannelId = PrivateMessageChannelId;
    privateChannel ??= await ModmailBot.This.Client.GetChannelAsync(privateChannelId);
    if (privateChannel is not null) {
      await privateChannel.SendMessageAsync(UserResponses.TicketTypeChanged(ticketType));
      if (BotTicketCreatedMessageInDmId != 0) {
        privateMessageWithComponent ??= await privateChannel.GetMessageAsync(BotTicketCreatedMessageInDmId);
        if (privateMessageWithComponent is not null) {
          //remove components from private messageContent
          await privateMessageWithComponent.ModifyAsync(x => {
            x.ClearComponents();
            x.AddEmbeds(privateMessageWithComponent.Embeds);
          });

          TicketTypeSelectionTimeoutMgr.This.RemoveMessage(privateMessageWithComponent.Id);
        }
      }
    }
    else {
      //TODO: Handle private channel not found
    }
  }
}