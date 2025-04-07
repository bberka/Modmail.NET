namespace Modmail.NET.Common.Extensions;

public static class StringExtensions
{
  public static string GetStringOrNaN(this string value) {
    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value)) return "NaN";

    return value;
  }
}