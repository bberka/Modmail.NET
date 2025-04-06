namespace Modmail.NET.Utils;

public static class UtilAttachment
{
  public static string GetUri(Guid id) {
    var botConfig = ServiceLocator.GetBotConfig();
    if (string.IsNullOrEmpty(botConfig.Domain)) throw new InvalidOperationException();

    var uri = new Uri(botConfig.Domain, UriKind.Absolute);
    return uri + "attachments/" + id;
  }
}