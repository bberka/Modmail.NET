using DSharpPlus.Commands;
using DSharpPlus.Commands.ContextChecks;
using Modmail.NET.Checks.Attributes;
using Modmail.NET.Utils;

namespace Modmail.NET.Checks;

public class RequireTicketChannelCheck : IContextCheck<RequireTicketChannelAttribute>
{
  public async ValueTask<string> ExecuteCheckAsync(RequireTicketChannelAttribute attribute, CommandContext context) {
    var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(context.Channel.Topic);
    if (ticketId != Guid.Empty) return null;

    return await Task.FromResult(LangKeys.ThisCommandCanOnlyBeUsedInTicketChannel.GetTranslation());
  }
}