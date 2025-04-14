using Modmail.NET.Common.Exceptions;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Utils;

public static class UtilInteraction
{
  private const string SplitText = "%&%;";

  public static string BuildKey(string name, params object[] parameters) {
    return $"{name}{SplitText}{string.Join(SplitText, parameters)}";
  }

  public static (string name, string[] parameters) ParseKey(string key) {
    if (string.IsNullOrEmpty(key)) throw new ModmailBotException(Lang.InvalidInteractionKey);
    var split = key.Split(SplitText);
    var name = split[0];
    var parameters = split[1..];
    return (name, parameters);
  }
}