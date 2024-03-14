using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using Modmail.NET.Common;
using Modmail.NET.Database;
using Serilog;

namespace Modmail.NET.Handlers;

public static class MailHandler
{
  public static async Task HandlePrivateMessage(DiscordClient sender,
                                                DiscordMessage message,
                                                DiscordChannel channel,
                                                DiscordUser author) {
    if (message.Author.IsBot) return;
    if (message.IsTTS) return;

    var channelId = (long)channel.Id;
    var authorId = (long)author.Id;
    var messageContent = message.Content;
    var attachments = message.Attachments;

    //Check if user has 
    await using var db = new ModmailDbContext();
    var option = await db.ModmailOptions
                         .AsQueryable()
                         .Where(x => x.GuildId == EnvContainer.This.MainServerId)
                         .FirstOrDefaultAsync();

    if (option is null) {
      Log.Error("ModmailOption not found for guild {GuildId}, server needs to be setup first", EnvContainer.This.MainServerId);
      return;
    }

    var existingLog = await db.ModmailLogs
                              .AsQueryable()
                              .Where(x => x.DiscordUserId == authorId)
                              .FirstOrDefaultAsync();

    var guild = await sender.GetGuildAsync((ulong)option.GuildId);
    var logChannel = guild.GetChannel((ulong)option.LogChannelId);


    if (existingLog is null) {
      //make new channel
      var channelName = $"modmail-{author.Username.Trim()}";
      var category = guild.GetChannel((ulong)option.CategoryId);
      var mailChannel = await guild.CreateTextChannelAsync(channelName, category);
      await mailChannel.SendMessageAsync(messageContent);
    }
    else {
      //continue on existing channel
      var mailChannel = guild.GetChannel((ulong)existingLog.ModMessageChannelId);
      await mailChannel.SendMessageAsync(messageContent);
    }
  }
}