using Modmail.NET.Entities;

namespace Modmail.NET.Attributes;

public class UpdateUserInformationForCommandAttribute : CheckBaseAttribute
{
    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        await DiscordUserInfo.AddOrUpdateAsync(ctx?.User);
        return true;
    }
}