using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Modmail.NET.Static;
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

  public async Task CloseTicketAsync(ulong closerUserId,
                                     string? closeReason = null,
                                     DiscordChannel? modChatChannel = null) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    // if (CloserUserInfo is null) throw new InvalidOperationException("CloserUserInfo is null");
    if (closerUserId == 0) throw new InvalidOperationException("CloserUserId is 0");
    if (string.IsNullOrEmpty(CloseReason)) CloseReason = Texts.NO_REASON_PROVIDED;
    if (ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket is already closed");


    if (modChatChannel is null) {
      modChatChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    }

    ClosedDateUtc = DateTime.UtcNow;
    CloseReason = closeReason;
    CloserUserId = closerUserId;

    await this.UpdateAsync();

    await modChatChannel.DeleteAsync(Texts.TICKET_CLOSED);

    var pmChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (pmChannel != null) {
      // var embed = ModmailEmbeds.ToUser.TicketClosed(this);
      var ticketClosedMsgToUser = new DiscordEmbedBuilder()
                                  .WithTitle(Texts.YOUR_TICKET_HAS_BEEN_CLOSED)
                                  .WithDescription(Texts.YOUR_TICKET_HAS_BEEN_CLOSED_DESCRIPTION)
                                  .WithFooter(GuildOption.Name, iconUrl: GuildOption.IconUrl)
                                  .WithTimestamp(DateTime.Now)
                                  .WithColor(ModmailEmbeds.TicketClosedColor);
      if (!string.IsNullOrEmpty(GuildOption.ClosingMessage))
        ticketClosedMsgToUser.WithDescription(GuildOption.ClosingMessage);
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
                                  .WithTimestamp(DateTime.Now)
                                  .WithFooter(GuildOption.Name, GuildOption.IconUrl)
                                  .WithColor(ModmailEmbeds.FeedbackColor);

        var response = ticketFeedbackMsgToUser
                       .AddEmbed(ticketFeedbackEmbed)
                       .AddComponents(starList);

        await pmChannel.SendMessageAsync(response);
      }
    }
    else {
      //TODO: Handle private message privateChannel not found
    }

    var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);
    if (logChannel != null) {
      // var embed = ModmailEmbeds.ToLog.TicketClosed(this);


      var embed = new DiscordEmbedBuilder()
                  // .WithDescription("Ticket has been closed.")
                  .WithTimestamp(DateTime.Now)
                  .WithTitle(Texts.TICKET_CLOSED)
                  .WithAuthor(CloserUserInfo.Username, iconUrl: CloserUserInfo.AvatarUrl)
                  .WithColor(ModmailEmbeds.TicketClosedColor)
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

    Result<Ticket>.Ok(this, Texts.TICKET_CLOSED_SUCCESSFULLY);
  }

  public async Task ChangePriority(ulong modUserId,
                                   TicketPriority newPriority,
                                   DiscordChannel? ticketChannel = null) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (modUserId == 0) throw new InvalidOperationException("ModUserId is 0");
    if (ClosedDateUtc.HasValue) throw new InvalidOperationException("Ticket is already closed");

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
                                           .WithTimestamp(DateTime.Now)
                                           .WithColor(ModmailEmbeds.TicketPriorityChangedColor)
                                           .AddField(Texts.OLD_PRIORITY, oldPriority.ToString(), true)
                                           .AddField(Texts.NEW_PRIORITY, newPriority.ToString(), true);
      if (!Anonymous) ticketPriorityChangedMsgToUser.WithAuthor(modUser.Username, iconUrl: modUser.AvatarUrl);
      await privateChannel.SendMessageAsync(ticketPriorityChangedMsgToUser);
    }
    else {
      //TODO: Handle private message privateChannel not found
    }


    var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);
    if (logChannel is not null) {
      var ticketPriorityChangedMsgToLog = new DiscordEmbedBuilder()
                                          .WithTitle(Texts.TICKET_PRIORITY_CHANGED)
                                          .WithTimestamp(DateTime.Now)
                                          .WithColor(ModmailEmbeds.TicketPriorityChangedColor)
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
                                           .WithColor(ModmailEmbeds.TicketPriorityChangedColor)
                                           .WithTimestamp(DateTime.Now)
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


  public async Task ProcessUserSentMessageAsync(DiscordUser user, DiscordChannel privateChannel, DiscordMessage message) {
    if (user is null) throw new InvalidOperationException("User is null");
    if (privateChannel is null) throw new InvalidOperationException("PrivateChannel is null");
    if (message is null) throw new InvalidOperationException("Message is null");
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");

    await Task.Delay(50); //wait for privateChannel creation process to finish

    LastMessageDateUtc = DateTime.UtcNow;
    await this.UpdateAsync();

    var mailChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (mailChannel is not null) {
      var embed = ModmailEmbeds.ToMail.MessageReceived(user, message);
      await mailChannel.SendMessageAsync(embed);
    }
    else {
      //TODO: Handle mail privateChannel not found
    }

    var ticketMessage = UtilMapper.DiscordMessageToEntity(message, Id);
    await ticketMessage.AddAsync();

    var embedUserMessageDelivered = ModmailEmbeds.ToUser.MessageSent(GuildOption.Name, GuildOption.IconUrl, user, message);
    await privateChannel.SendMessageAsync(embedUserMessageDelivered);

    if (GuildOption.IsSensitiveLogging) {
      var logChannel = await ModmailBot.This.Client.GetChannelAsync(GuildOption.LogChannelId);

      if (logChannel is not null) {
        var embed3 = ModmailEmbeds.ToLog.MessageSentByUser(user,
                                                           message,
                                                           Id);
        await logChannel.SendMessageAsync(embed3);
      }
      else {
        //TODO: Handle log privateChannel not found
      }
    }
  }

  public static async Task ProcessCreateNewTicketAsync(DiscordUser user, DiscordChannel privateChannel, DiscordMessage message) {
    var guildOption = await Entities.GuildOption.GetAsync();
    if (guildOption is null) throw new Exception("GuildOption not found");

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


    var newTicketMessageBuilder = ModmailEmbeds.ToMail.NewTicket(member, ticketId, modRoleListForOverwrites, modMemberListForOverwrites);
    await mailChannel.SendMessageAsync(newTicketMessageBuilder);

    var embedUserMessage = ModmailEmbeds.ToMail.MessageReceived(user, message);
    await mailChannel.SendMessageAsync(embedUserMessage);

    var ticketMessage = UtilMapper.DiscordMessageToEntity(message, ticketId);

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
      }
    };


    await ticket.AddAsync();

    var ticketTypes = await Entities.TicketType.GetAllAsync();

    var embedTicketCreated = ModmailEmbeds.ToUser.TicketCreated(guild,
                                                                message,
                                                                guildOption,
                                                                ticketTypes,
                                                                ticketId);
    var embedUserMessageSentToUser = ModmailEmbeds.ToUser.MessageSent(guildOption.Name, guildOption.IconUrl, user, message);

    var ticketCreatedMessage = await privateChannel.SendMessageAsync(embedTicketCreated);
    TicketTypeSelectionTimeoutMgr.This.AddMessage(ticketCreatedMessage);
    await privateChannel.SendMessageAsync(embedUserMessageSentToUser);

    var newTicketCreatedLog = ModmailEmbeds.ToLog.NewTicketCreated(user, message, mailChannel, ticket.Id);
    var logChannel = await ModmailBot.This.Client.GetChannelAsync(guildOption.LogChannelId);

    if (logChannel is not null) {
      await logChannel.SendMessageAsync(newTicketCreatedLog);

      if (guildOption.IsSensitiveLogging) {
        var messageSentByUserLog = ModmailEmbeds.ToLog.MessageSentByUser(user,
                                                                         message,
                                                                         ticket.Id);
        await logChannel.SendMessageAsync(messageSentByUserLog);
      }
    }
  }

  public async Task ProcessModSendMessageAsync(DiscordUser modUser,
                                               DiscordMessage message,
                                               DiscordChannel channel,
                                               DiscordGuild guild) {
    if (GuildOption is null) throw new InvalidOperationException("GuildOption is null");
    if (OpenerUserInfo is null) throw new InvalidOperationException("OpenerUserInfo is null");
    if (modUser is null) throw new InvalidOperationException("ModUser is null");
    if (message is null) throw new InvalidOperationException("Message is null");
    if (channel is null) throw new InvalidOperationException("Channel is null");


    // var user = await ModmailBot.This.GetMemberFromAnyGuildAsync(ticket.OpenerUserId);
    // if (user is null) {
    //   Log.Error("Member not found for user: {UserId}", ticket.OpenerUserId);
    //   return;
    // }

    var privateChannel = await ModmailBot.This.Client.GetChannelAsync(PrivateMessageChannelId);
    if (privateChannel is not null) {
      var embed = ModmailEmbeds.ToUser.MessageReceived(modUser, message, guild, Anonymous);
      await privateChannel.SendMessageAsync(embed);
    }

    var ticketChannel = await ModmailBot.This.Client.GetChannelAsync(ModMessageChannelId);
    if (ticketChannel is not null) {
      var embed2 = ModmailEmbeds.ToMail.MessageSent(modUser, message, Anonymous);
      await ticketChannel.SendMessageAsync(embed2);
      await message.DeleteAsync();
    }


    LastMessageDateUtc = DateTime.UtcNow;
    await UpdateAsync();


    if (GuildOption.IsSensitiveLogging) {
      var ticketMessage = UtilMapper.DiscordMessageToEntity(message, Id);
      await ticketMessage.AddAsync();


      var logChannelId = GuildOption.LogChannelId;
      var logChannel = guild.GetChannel(logChannelId);
      var embed3 = ModmailEmbeds.ToLog.MessageSentByMod(modUser,
                                                        OpenerUserInfo,
                                                        message,
                                                        channel,
                                                        Id,
                                                        Anonymous);
      await logChannel.SendMessageAsync(embed3);
    }
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


  public async Task ProcessInsertFeedbackAsync(int starCount, string textInput, DiscordMessage feedbackMessage) {
    if (starCount < 1 || starCount > 5) {
      throw new InvalidOperationException("Star count must be between 1 and 5");
    }

    if (GuildOption.TakeFeedbackAfterClosing) {
      throw new InvalidOperationException("Feedback is not enabled for this guild: " + GuildOptionId);
    }

    if (string.IsNullOrEmpty(textInput)) {
      throw new InvalidOperationException("Feedback message is empty");
    }

    if (feedbackMessage is null) {
      throw new InvalidOperationException("Feedback message is null");
    }

    FeedbackStar = starCount;
    FeedbackMessage = textInput;
    await UpdateAsync();

    var mainGuild = await ModmailBot.This.GetMainGuildAsync();

    var feedbackDone = new DiscordEmbedBuilder()
                       .WithTitle(Texts.FEEDBACK_RECEIVED)
                       .WithTimestamp(DateTime.Now)
                       .WithFooter(GuildOption.Name, GuildOption.IconUrl)
                       .AddField(Texts.STAR, Texts.STAR_EMOJI + starCount)
                       .AddField(Texts.FEEDBACK, textInput)
                       .WithColor(ModmailEmbeds.FeedbackColor);

    await feedbackMessage.ModifyAsync(x => { x.AddEmbed(feedbackDone); });

    var logChannel = mainGuild.GetChannel(GuildOption.LogChannelId);
    if (logChannel is not null) {
      var logEmbed = new DiscordEmbedBuilder()
                     .WithTitle(Texts.FEEDBACK_RECEIVED)
                     .WithDescription(textInput)
                     .WithTimestamp(DateTime.Now)
                     .WithFooter(mainGuild.Name, mainGuild.IconUrl)
                     .AddField(Texts.STAR, starCount.ToString(), true)
                     .AddField(Texts.USER, OpenerUserInfo.GetMention(), true)
                     .AddField(Texts.TICKET_ID, Id.ToString().ToUpper(), true)
                     .WithColor(ModmailEmbeds.FeedbackColor)
                     .WithAuthor(OpenerUserInfo.Username, iconUrl: OpenerUserInfo.AvatarUrl);
      await logChannel.SendMessageAsync(logEmbed);
    }
  }
}