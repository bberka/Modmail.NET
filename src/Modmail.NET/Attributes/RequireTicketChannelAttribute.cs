using DSharpPlus;
using DSharpPlus.SlashCommands;
using Modmail.NET.Common;
using Modmail.NET.Static;
using Modmail.NET.Utils;

namespace Modmail.NET.Attributes;

public class RequireTicketChannelAttribute : SlashCheckBaseAttribute
{
  public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx) {
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    if (ticketId != Guid.Empty) {
      return true;
    }

    await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, Interactions.Error(Texts.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL));
    return false;
  }
}