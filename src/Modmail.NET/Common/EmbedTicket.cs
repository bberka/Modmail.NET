using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using Modmail.NET.Entities;
using Modmail.NET.Extensions;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Common;

public static class EmbedTicket
{
  public static DiscordEmbedBuilder YouHaveBeenBlacklisted(DiscordUser? author = null, string? reason = null) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOU_HAVE_BEEN_BLACKLISTED)
                .WithDescription(Texts.YOU_HAVE_BEEN_BLACKLISTED_DESCRIPTION)
                .WithGuildInfoFooter()
                .WithCustomTimestamp()
                .WithColor(Colors.ErrorColor);
    if (author is not null) {
      embed.WithAuthor(author.GetUsername(), iconUrl: author.AvatarUrl);
    }

    if (!string.IsNullOrEmpty(reason)) {
      embed.AddField(Texts.REASON, reason);
    }

    return embed;
  }


  public static DiscordMessageBuilder YouHaveCreatedNewTicket(DiscordGuild guild,
                                                              GuildOption option,
                                                              List<TicketType> ticketTypes,
                                                              Guid ticketId) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.YOU_HAVE_CREATED_NEW_TICKET)
                .WithFooter(guild.Name, guild.IconUrl)
                .WithCustomTimestamp()
                .WithColor(Colors.TicketCreatedColor);
    if (!string.IsNullOrEmpty(option.GreetingMessage))
      embed.WithDescription(option.GreetingMessage);

    var builder = new DiscordMessageBuilder()
      .AddEmbed(embed);

    if (ticketTypes.Count > 0) {
      var selectBox = new DiscordSelectComponent(UtilInteraction.BuildKey("ticket_type", ticketId.ToString()),
                                                 Texts.PLEASE_SELECT_A_TICKET_TYPE,
                                                 ticketTypes.Select(x => new DiscordSelectComponentOption(x.Name, x.Key.ToString(), x.Description, false, new DiscordComponentEmoji(x.Emoji)))
                                                            .ToList());
      builder.AddComponents(selectBox);
    }

    return builder;
  }


  public static DiscordMessageBuilder NewTicket(DiscordUser member, Guid ticketId, List<DiscordRole> modRoleListForOverwrites, List<DiscordMember> modMemberListForOverwrites) {
    var embed = new DiscordEmbedBuilder()
                .WithTitle(Texts.NEW_TICKET)
                .WithCustomTimestamp()
                .WithDescription(Texts.NEW_TICKET_DESCRIPTION_MESSAGE)
                .WithAuthor(member.GetUsername(), iconUrl: member.AvatarUrl)
                .AddField(Texts.USER, member.Mention, true)
                .AddField(Texts.TICKET_ID, ticketId.ToString().ToUpper(), true)
                .WithColor(Colors.TicketCreatedColor);

    var messageBuilder = new DiscordMessageBuilder()
                         .AddEmbed(embed)
                         .AddComponents(new DiscordButtonComponent(ButtonStyle.Danger,
                                                                   UtilInteraction.BuildKey("close_ticket", ticketId.ToString()),
                                                                   Texts.CLOSE_TICKET,
                                                                   emoji: new DiscordComponentEmoji("🔒"))
                                        // ,
                                        // new DiscordButtonComponent(ButtonStyle.Danger,
                                        //                            UtilInteraction.BuildKey("close_ticket_with_reason", ticketId.ToString()),
                                        //                            Texts.CLOSE_TICKET_WITH_REASON,
                                        //                            emoji: new DiscordComponentEmoji("🔒"))
                                       );

    var sb = new StringBuilder();
    foreach (var role in modRoleListForOverwrites) sb.AppendLine(role.Mention);
    foreach (var member2 in modMemberListForOverwrites) sb.AppendLine(member2.Mention);
    messageBuilder.WithContent(sb.ToString());

    return messageBuilder;
  }
}