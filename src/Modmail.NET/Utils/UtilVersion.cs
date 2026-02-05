namespace Modmail.NET.Utils;

public static class UtilVersion
{
    public static string GetVersion()
    {
        var currentAssembly = typeof(UtilVersion).Assembly;
        var version = currentAssembly.GetName()
            .Version;
        return version?.ToString() ?? "Unknown";
    }
}