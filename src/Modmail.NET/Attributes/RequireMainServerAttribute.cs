namespace Modmail.NET.Attributes;

public class RequireMainServerForSlashCommandAttribute : SlashCheckBaseAttribute
{
    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var isMainServer = BotConfig.This.MainServerId == ctx.Guild.Id;
        if (isMainServer) return true;

        await ctx.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, Interactions
            .Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation())
            .AsEphemeral());
        return false;
    }
}