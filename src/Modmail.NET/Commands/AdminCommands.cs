using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Modmail.NET.Common;
using Modmail.NET.Database;

namespace Modmail.NET.Commands;

[RequireBotPermissions(Permissions.SendMessages)]
public class AdminCommands : BaseCommandModule
{

  [Command("close")]
  [Description("Close a ticket.")]
  [RequireUserPermissions(Permissions.Administrator)]
  public async Task CloseTicket(CommandContext ctx, string reason = "")
  {
    var currentChannel = ctx.Channel;
    var currentGuild = ctx.Guild;
    var currentMessage = ctx.Message;
    var currentMember = ctx.Member;
    var currentUser = ctx.User;
    var channelTopic = currentChannel.Topic;
    
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(channelTopic);
    if (ticketId == Guid.Empty) {
      await currentChannel.SendMessageAsync("This command can only be used in a ticket channel.");
      await Task.Delay(3000);
      await currentMessage.DeleteAsync();
      return;
    }
    
    var db = new ModmailDbContext();
    
    var ticket = await db.GetActiveModmailAsync(ticketId);
    if (ticket is null) {
      await currentChannel.SendMessageAsync("This command can only be used in an active ticket channel.");
      await Task.Delay(3000);
      await currentMessage.DeleteAsync();
      return;
    }

    var logChannel = await ModmailBot.This.GetLogChannelAsync();
    var ticketOpenUser = await currentGuild.GetMemberAsync(ticket.DiscordUserId);
    var logEmbed = ModmailEmbedBuilder.ToLog.TicketClosed(currentUser,ticketOpenUser, ticketId,ticket.RegisterDate,reason);
    await logChannel.SendMessageAsync(embed: logEmbed);

    var embed = ModmailEmbedBuilder.ToUser.TicketClosed(ctx.Guild,ticketOpenUser);
    await ticketOpenUser.SendMessageAsync(embed);
    
    
    await currentChannel.DeleteAsync();
    
    ticket.ClosedDate = DateTime.Now;
    await db.SaveChangesAsync();
  }
   
}