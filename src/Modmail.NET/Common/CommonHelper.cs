namespace Modmail.NET.Common;

public static class CommonHelper
{
  public static string GetStringOrNA(this string? value) {
    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) return "N/A";

    return value;
  }
}