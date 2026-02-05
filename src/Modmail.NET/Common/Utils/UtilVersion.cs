using System.Diagnostics;

namespace Modmail.NET.Common.Utils;

public static class UtilVersion
{
  public static string GetVersion() {
    var currentAssembly = typeof(UtilVersion).Assembly;
    var version = currentAssembly.GetName().Version;
    return version?.ToString() ?? "Unknown";
  }

  public static string GetProductVersion() {
    var fileVersionInfo = FileVersionInfo.GetVersionInfo(typeof(UtilVersion).Assembly.Location);
    var version = fileVersionInfo.ProductVersion;
    return version;
  }

  public static string GetReadableProductVersion() {
    var fileVersionInfo = FileVersionInfo.GetVersionInfo(typeof(UtilVersion).Assembly.Location);
    var version = fileVersionInfo.ProductVersion;
    var split = version?.Split("+") ?? [];
    if (split.Length == 2) {
      var first = split[0];
      return first;
    }

    return version;
  }
}