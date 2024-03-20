namespace Modmail.NET.Common;

public static class UtilInteraction
{
  private const string SPLIT_TEXT = "%&%;";

  public static string BuildKey(string name, params object?[] parameters) {
    return $"{name}{SPLIT_TEXT}{string.Join(SPLIT_TEXT, parameters)}";
  }

  public static (string name, string[] parameters) ParseKey(string key) {
    var split = key.Split(SPLIT_TEXT);
    var name = split[0];
    var parameters = split[1..];
    return (name, parameters);
  }
}