using Serilog;

namespace Modmail.NET.Utils;

public static class UtilChannelTopic

{
  public static string BuildChannelTopic(Guid mailId) {
    return $"DO NOT CHANGE THIS || {mailId.ToString().ToUpper()} || {UtilDate.GetNow():O}";
  }

  public static Guid GetTicketIdFromChannelTopic(string topic) {
    try {
      if (string.IsNullOrEmpty(topic)) return Guid.Empty;
      var split = topic.Split("||");
      if (split.Length != 3) return Guid.Empty;
      return Guid.Parse(split[1].Trim());
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to parse mail id from channel topic");
      return Guid.Empty;
    }
  }
}