namespace Modmail.NET.Attributes;

public class RequireTicketChannelForSlashAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var ticketId = UtilChannelTopic.GetTicketIdFromChannelTopic(ctx.Channel.Topic);
        if (ticketId != Guid.Empty) return true;

        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, Interactions
            .Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_TICKET_CHANNEL.GetTranslation())
            .AsEphemeral());
        return false;
    }
}