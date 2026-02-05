namespace Modmail.NET.Attributes;

public class RequireMainServerForCommandAttribute : CheckBaseAttribute
{
    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        var isMainServer = BotConfig.This.MainServerId == ctx.Guild.Id;
        if (isMainServer) return Task.FromResult(true);

        ctx.RespondAsync(Embeds.Error(LangKeys.THIS_COMMAND_CAN_ONLY_BE_USED_IN_MAIN_SERVER.GetTranslation()));
        return Task.FromResult(false);
    }
}