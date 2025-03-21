using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Modmail.NET.Utils;

namespace Modmail.NET.Attributes;

public sealed class RequireTicketChannelForCommandAttribute : CheckBaseAttribute
{
  public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help) {
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
    if (ticketId != Guid.Empty) return true;

    await ctx.RespondAsync(Embeds.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL.GetTranslation()));
    return false;
  }
}