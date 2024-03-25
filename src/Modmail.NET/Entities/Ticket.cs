using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Exceptions;
using Modmail.NET.Extensions;
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


  public static async Task<Ticket?> GetActiveAsync(ulong userId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets
                                .FirstOrDefaultAsync(x => x.OpenerUserId == userId && !x.ClosedDateUtc.HasValue);
    return ticket;
  }

  public static async Task<Ticket?> GetActiveAsync(Guid ticketId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId && !x.ClosedDateUtc.HasValue);
    return ticket;
  }

  public static async Task<Ticket?> GetAsync(Guid id) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == id);
    return ticket;
  }

  public static async Task<Ticket?> GetClosedAsync(Guid ticketId) {
    await using var dbContext = ServiceLocator.Get<ModmailDbContext>();
    var ticket = await dbContext.Tickets.FirstOrDefaultAsync(x => x.Id == ticketId && x.ClosedDateUtc.HasValue);
    return ticket;
  }


  public async Task ProcessCloseTicketAsync(ulong closerUserId,
                                            string? closeReason = null,
                                            DiscordChannel? modChatChannel = null) {
    ArgumentNullException.ThrowIfNull(GuildOption);
    ArgumentNullException.ThrowIfNull(OpenerUserInfo);
    if (closerUserId == 0) throw new InvalidUserIdException();
    if (string.IsNullOrEmpty(CloseReason)) CloseReason = Texts.NO_REASON_PROVIDED;
    if (ClosedDateUtc.HasValue) throw new TicketAlreadyClosedException();
    CloserUserInfo = await DiscordUserInfo.GetAsync(closerUserId);
    ArgumentNullException.ThrowIfNull(CloserUserInfo);

    modChatChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);

    ClosedDateUtc = DateTime.UtcNow;
    CloseReason = closeReason;
    CloserUserId = closerUserId;

    await this.UpdateAsync();

    await modChatChannel.DeleteAsync(Texts.TICKET_CLOSED);

    var pmChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (pmChannel != null) {
      // var embed = Colors.ToUser.TicketClosed(this);
      var ticketClosedMsgToUser = new DiscordEmbedBuilder()
                                  .WithTitle(Texts.YOUR_TICKET_HAS_BEEN_CLOSED)
                                  .WithDescription(Texts.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION)
                                  .WithFooter(GuildOption.Name, iconUrl: GuildOption.IconUrl)
                                  .WithCustomTimestamp()
                                  .WithColor(Colors.TicketClosedColor);
      if (!string.IsNullOrEmpty(GuildOption.ClosingMessage))
        ticketClosedMsgToUser.WithDescription(GuildOption.ClosingMessage);

      if (!Anonymous) {
        ticketClosedMsgToUser.WithAuthor(CloserUserInfo.Username, iconUrl: CloserUserInfo.AvatarUrl);
      }

      await pmChannel.SendMessageAsync(ticketClosedMsgToUser);

      if (GuildOption.TakeFeedbackAfterClosing) {
        var ticketFeedbackMsgToUser = new DiscordMessageBuilder();
        var starList = new List<DiscordComponent> {
          new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 1, Id), "1", false, new DiscordComponentEmoji("⭐")),
          new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 2, Id), "2", false, new DiscordComponentEmoji("⭐")),
          new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 3, Id), "3", false, new DiscordComponentEmoji("⭐")),
          new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 4, Id), "4", false, new DiscordComponentEmoji("⭐")),
          new DiscordButtonComponent(ButtonStyle.Primary, UtilInteraction.BuildKey("star", 5, Id), "5", false, new DiscordComponentEmoji("⭐"))
        };

        var ticketFeedbackEmbed = new DiscordEmbedBuilder()
                                  .WithTitle(Texts.FEEDBACK)
                                  .WithDescription(Texts.FEEDBACK_DESCRIPTION)
                                  .WithCustomTimestamp()
                                  .WithFooter(GuildOption.Name, GuildOption.IconUrl)
                                  .WithColor(Colors.FeedbackColor);

        var response = ticketFeedbackMsgToUser
                       .AddEmbed(ticketFeedbackEmbed)
                       .AddComponents(starList);

        await pmChannel.SendMessageAsync(response);
      }
    }
    else {
      //TODO: Handle private messageContent privateChannel not found
    }

    var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);
    if (logChannel != null) {
      // var embed = Colors.ToLog.TicketClosed(this);


      var embed = new DiscordEmbedBuilder()
                  // .WithDescription("Ticket has been closed.")
                  .WithCustomTimestamp()
                  .WithTitle(Texts.TICKET_CLOSED)
                  .WithAuthor(CloserUserInfo.Username, iconUrl: CloserUserInfo.AvatarUrl)
                  .WithColor(Colors.TicketClosedColor)
                  .AddField(Texts.TICKET_ID, Id.ToString().ToUpper(), false)
                  .AddField(Texts.OPENED_BY, OpenerUserInfo.GetMention(), true)
                  .AddField(Texts.CLOSED_BY, CloserUserInfo.GetMention(), true)
                  .AddField(Texts.CLOSE_REASON, CloseReason, true);

      await logChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle log privateChannel not found
    }

    Log.Information("Ticket closed {TicketId} by {CloserUserId}", Id, closerUserId);
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
      var ticketPriorityChangedMsgToUser = new DiscordEmbedBuilder()
                                           .WithFooter(GuildOption.Name, GuildOption.IconUrl)
                                           .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                                           .WithCustomTimestamp()
                                           .WithColor(Colors.TicketPriorityChangedColor)
                                           .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                                           .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
      if (!Anonymous) ticketPriorityChangedMsgToUser.WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      await privateChannel.SendMessageAsync(ticketPriorityChangedMsgToUser);
    }
    else {
      //TODO: Handle private messageContent privateChannel not found
    }


    var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);
    if (logChannel is not null) {
      var ticketPriorityChangedMsgToLog = new DiscordEmbedBuilder()
                                          .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                                          .WithCustomTimestamp()
                                          .WithColor(Colors.TicketPriorityChangedColor)
                                          .AddField(Texts.TICKET_ID, Id.ToString().ToUpper())
                                          .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                                          .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
      if (!Anonymous) ticketPriorityChangedMsgToLog.WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      await logChannel.SendMessageAsync(ticketPriorityChangedMsgToLog);
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

      var ticketPriorityChangedMsgToMail = new DiscordEmbedBuilder()
                                           .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                                           .WithColor(Colors.TicketPriorityChangedColor)
                                           .WithCustomTimestamp()
                                           .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                                           .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true)
                                           .WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      await ticketChannel.SendMessageAsync(ticketPriorityChangedMsgToMail);
    }
    else {
      //TODO: Handle ticket privateChannel not found
    }

    Log.Information("Priority changed {TicketId} by {GuildId}", Id, modUserId);
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
      var embed = new DiscordEmbedBuilder()
                  .WithColor(Colors.MessageReceivedColor)
                  .WithUserAsAuthor(user)
                  .WithDescription(message.Content)
                  .WithCustomTimestamp()
                  .WithGuildInfoFooter()
                  .AddAttachment(message.Attachments);
      await mailChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle mail privateChannel not found
    }

    var ticketMessage = TicketMessage.MapFrom(Id, message);
    await ticketMessage.AddAsync();


    var embedUserMessageDelivered = new DiscordEmbedBuilder()
                                    .WithGuildInfoFooter(GuildOption)
                                    .WithDescription(message.Content)
                                    .WithCustomTimestamp()
                                    .WithUserAsAuthor(user)
                                    .WithColor(Colors.MessageSentColor)
                                    .AddAttachment(message.Attachments);

    await privateChannel.SendMessageAsync(embedUserMessageDelivered);

    if (GuildOption.IsSensitiveLogging) {
      var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);

      if (logChannel is not null) {
        var embed = new DiscordEmbedBuilder()
                    .WithTitle(Texts.MESSAGE_SENT_BY_USER)
                    .WithUserAsAuthor(user)
                    .WithDescription(message.Content)
                    .WithCustomTimestamp()
                    .WithColor(Colors.MessageSentColor)
                    .AddField(Texts.TICKET_ID, Id.ToString().ToUpper())
                    .AddField(Texts.USER_ID, user.Id.ToString(), true)
                    .AddAttachment(message.Attachments);
        await logChannel.SendMessageAsync(embed);
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


    var newTicketMessageBuilder = EmbedTicket.NewTicket(member, ticketId, modRoleListForOverwrites, modMemberListForOverwrites);
    await mailChannel.SendMessageAsync(newTicketMessageBuilder);
    var embedUserMessage = new DiscordEmbedBuilder()
                           .WithColor(Colors.MessageReceivedColor)
                           .WithUserAsAuthor(user)
                           .WithDescription(message.Content)
                           .WithCustomTimestamp()
                           .WithGuildInfoFooter()
                           .AddAttachment(message.Attachments);
    await mailChannel.SendMessageAsync(embedUserMessage);

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

    var ticketTypes = await Entities.TicketType.GetAllAsync();

    var embedTicketCreated = EmbedTicket.YouHaveCreatedNewTicket(guild,
                                                                 guildOption,
                                                                 ticketTypes,
                                                                 ticketId);


    var embedUserMessageSentToUser = new DiscordEmbedBuilder()
                                     .WithColor(Colors.MessageSentColor)
                                     .WithUserAsAuthor(user)
                                     .WithDescription(message.Content)
                                     .WithCustomTimestamp()
                                     .WithGuildInfoFooter()
                                     .AddAttachment(message.Attachments);


    var ticketCreatedMessage = await privateChannel.SendMessageAsync(embedTicketCreated);
    TicketTypeSelectionTimeoutMgr.This.AddMessage(ticketCreatedMessage);
    var dmTicketCreatedMessage = await privateChannel.SendMessageAsync(embedUserMessageSentToUser);

    ticket.BotTicketCreatedMessageInDmId = dmTicketCreatedMessage.Id;
    await ticket.UpdateAsync();

    var newTicketCreatedLog = EmbedLog.NewTicketCreated(message, mailChannel, ticket.Id);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(guildOption.LogChannelId);

    if (logChannel is not null) {
      await logChannel.SendMessageAsync(newTicketCreatedLog);

      if (guildOption.IsSensitiveLogging) {
        var embed = new DiscordEmbedBuilder()
                    .WithTitle(Texts.MESSAGE_SENT_BY_USER)
                    .WithUserAsAuthor(user)
                    .WithDescription(message.Content)
                    .WithCustomTimestamp()
                    .WithColor(Colors.MessageSentColor)
                    .AddField(Texts.TICKET_ID, ticket.Id.ToString().ToUpper())
                    .AddField(Texts.USER_ID, user.Id.ToString(), true)
                    .AddAttachment(message.Attachments);
        await logChannel.SendMessageAsync(embed);
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

    // var user = await ModmailBot.This.GetMemberFromAnyGuildAsync(ticket.messageId);
    // if (user is null) {
    //   Log.Error("Member not found for user: {UserId}", ticket.messageId);
    //   return;
    // }

    var privateChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (privateChannel is not null) {
      var embed = EmbedMessage.MessageReceivedFromMod(message, Anonymous);
      await privateChannel.SendMessageAsync(embed);
    }

    var ticketChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      var embed2 = EmbedMessage.MessageSentByMod(message, Anonymous);
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
      var embed3 = EmbedLog.MessageSentByMod(message,
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

    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(Texts.FEEDBACK_RECEIVED)
                       .WithCustomTimestamp()
                       .WithFooter(GuildOption.Name, GuildOption.IconUrl)
                       .AddField(Texts.STAR, Texts.STAR_EMOJI + starCount)
                       .AddField(Texts.FEEDBACK, textInput)
                       .WithColor(Colors.FeedbackColor);

    await feedbackMessage.ModifyAsync(x => { x.AddEmbed(feedbackDone); });

    var logChannel = mainGuild.GetChannel(GuildOption.LogChannelId);
    if (logChannel is not null) {
      var logEmbed = new DiscordEmbedBuilder()
                     .WithTitle(Texts.FEEDBACK_RECEIVED)
                     .WithDescription(textInput)
                     .WithCustomTimestamp()
                     .WithFooter(mainGuild.Name, mainGuild.IconUrl)
                     .AddField(Texts.TICKET_ID, Id.ToString().ToUpper(), false)
                     .AddField(Texts.USER, OpenerUserInfo.GetMention(), true)
                     .AddField(Texts.STAR, starCount.ToString(), true)
                     .WithColor(Colors.FeedbackColor)
                     .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl);
      await logChannel.SendMessageAsync(logEmbed);
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


    var logChannelId = await GuildOption.GetLogChannelIdAsync(GuildOption.GuildId);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(logChannelId);
    if (logChannel is not null) {
      var noteAddedColorEmbed = new DiscordEmbedBuilder()
                                .WithTitle(Texts.NOTE_ADDED)
                                .WithDescription(note)
                                .WithColor(Colors.NoteAddedColor)
                                .WithCustomTimestamp()
                                .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl)
                                .AddField(Texts.TICKET_ID, Id.ToString().ToUpper());
      await logChannel.SendMessageAsync(noteAddedColorEmbed);
    }
    else {
      //TODO: Handle log privateChannel not found
    }


    var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (mailChannel is not null) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.NOTE_ADDED)
                  .WithDescription(note)
                  .WithColor(Colors.NoteAddedColor)
                  .WithCustomTimestamp()
                  .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl);
      await mailChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle mail channel not found
    }

    Log.Information("Note added {TicketId} by {UserId}", Id, userId);
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
      var aToggledEmbed = new DiscordEmbedBuilder()
                          .WithTitle(Texts.ANONYMOUS_TOGGLED)
                          .WithColor(Colors.AnonymousToggledColor)
                          .WithCustomTimestamp()
                          .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl)
                          .AddField(Texts.TICKET_ID, Id.ToString().ToUpper());
      aToggledEmbed.WithDescription(Anonymous
                                      ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                                      : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);
      await logChannel.SendMessageAsync(aToggledEmbed);
    }
    else {
      //TODO: Handle log channel not found
    }

    ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      var embed2 = new DiscordEmbedBuilder()
                   .WithTitle(Texts.ANONYMOUS_TOGGLED)
                   .WithColor(Colors.AnonymousToggledColor)
                   .WithCustomTimestamp()
                   .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl)
                   .WithDescription(Anonymous
                                      ? Texts.TICKET_SET_ANONYMOUS_DESCRIPTION
                                      : Texts.TICKET_SET_NOT_ANONYMOUS_DESCRIPTION);

      await ticketChannel.SendMessageAsync(embed2);
    }
    else {
      //TODO: Handle mail channel not found
    }

    Log.Information("Anonymous mode toggled {TicketId} by {UserId}", Id, OpenerUserId);
  }

  public async Task ProcessChangeTicketTypeAsync(ulong userId,
                                                 string type,
                                                 DiscordChannel? ticketChannel = null,
                                                 DiscordChannel? privateChannel = null,
                                                 DiscordMessage? privateMessageWithComponent = null) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (userId == 0) throw new InvalidOperationException("UserId is 0");
    if (ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket is already closed");

    var ticketType = await Entities.TicketType.GetAsync(type);
    if (ticketType is null) throw new InvalidOperationException("TicketType is null");

    TicketTypeId = ticketType.Id;
    await UpdateAsync();

    var logChannelId = await GuildOption.GetLogChannelIdAsync(GuildOption.GuildId);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(logChannelId);
    if (logChannel is not null) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.TICKET_TYPE_CHANGED)
                  .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                  .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl)
                  .WithCustomTimestamp()
                  .AddField(Texts.TICKET_ID, Id.ToString().ToUpper())
                  .WithColor(Colors.TicketTypeChangedColor);
      await logChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle log channel not found
    }

    var userInfo = await DiscordUserInfo.GetAsync(userId);
    ticketChannel ??= await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.TICKET_TYPE_CHANGED)
                  .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                  .WithAuthor(userInfo.Username, iconUrl: userInfo.AvatarUrl)
                  .WithCustomTimestamp()
                  .WithColor(Colors.TicketTypeChangedColor);

      await ticketChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle mail channel not found
    }

    var privateChannelId = PrivateMessageChannelId;
    privateChannel ??= await ModmailBot.This.Client.GetChannelAsync(privateChannelId);
    if (privateChannel is not null) {
      var embed = new DiscordEmbedBuilder()
                  .WithTitle(Texts.TICKET_TYPE_CHANGED)
                  .WithDescription(string.Format(Texts.TICKET_TYPE_SET, ticketType.Emoji, ticketType.Name))
                  .WithCustomTimestamp()
                  .WithColor(Colors.TicketTypeChangedColor);
      if (!string.IsNullOrEmpty(ticketType.EmbedMessageTitle) && !string.IsNullOrEmpty(ticketType.EmbedMessageContent))
        embed.AddField(ticketType.EmbedMessageTitle, ticketType.EmbedMessageContent);
      await privateChannel.SendMessageAsync(embed);

      privateMessageWithComponent ??= await privateChannel.GetMessageAsync(BotTicketCreatedMessageInDmId);
      if (privateMessageWithComponent is not null) {
        //remove components from private messageContent
        var embedPmInteraction = privateMessageWithComponent.Embeds.FirstOrDefault();
        await privateMessageWithComponent.ModifyAsync(x => {
          x.ClearComponents();
          x.AddEmbed(embedPmInteraction);
        });

        TicketTypeSelectionTimeoutMgr.This.RemoveMessage(privateMessageWithComponent.Id);
      }
    }
    else {
      //TODO: Handle private channel not found
    }
  }
}