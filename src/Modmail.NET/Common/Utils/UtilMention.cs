using System.Text;
using Modmail.NET.Features.Teams.Models;

namespace Modmail.NET.Common.Utils;

public static class UtilMention
{
  //TODO: Check if ping notification works for the team users
  public static string GetNewTicketPingText(UserTeamInformation[] users) {
    var sb = new StringBuilder();
    foreach (var perm in users.Where(x => x.PingOnNewTicket)) sb.AppendLine(perm.GetMention());
    return sb.ToString();
  }

  public static string GetNewMessagePingText(UserTeamInformation[] users) {
    var sb = new StringBuilder();
    foreach (var perm in users.Where(x => x.PingOnNewMessage)) sb.AppendLine(perm.GetMention());
    return sb.ToString();
  }
}