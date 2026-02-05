namespace Modmail.NET.Features.Teams.Models;

public sealed record UserTeamInformation(
    ulong UserId,
    bool PingOnNewTicket,
    bool PingOnNewMessage)
{
    public string GetMention()
    {
        return $"<@{UserId}>";
    }
}