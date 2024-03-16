using Serilog;

namespace Modmail.NET.Common;

public static class UtilChannelTopic

{
  public static string BuildChannelTopic(Guid mailId) {
    return $"DO NOT CHANGE THIS || {mailId.ToString().ToUpper()} || {DateTime.UtcNow:O}";
  }

  public static Guid GetTicketIdFromChannelTopic(string topic) {
    try {
      if(string.IsNullOrEmpty(topic)) return Guid.Empty;
      var split = topic.Split("||");
      if(split.Length != 3) return Guid.Empty;
      return Guid.Parse(split[1].Trim());
    }
    catch (Exception ex) {
      Log.Error(ex, "Failed to parse mail id from channel topic");
      return Guid.Empty;
    }
  }
}