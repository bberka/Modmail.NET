namespace Modmail.NET.Extensions;

public static class ExtString
{
  public static string GetStringOrNaN(this string value) {
    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) return "NaN";

    return value;
  }
}