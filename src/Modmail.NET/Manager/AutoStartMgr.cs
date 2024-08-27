using System.Runtime.InteropServices;
using Microsoft.Win32;
using Serilog;

namespace Modmail.NET.Manager;

public static class AutoStartMgr
{
  /// <summary>
  /// Adds the application to the autostart list on operating system start
  /// </summary>
  public static void HandleAutomaticAppStart() {
    if (BotConfig.This.AddToAutoStart == false) return;
    var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
    var isLinux = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    if (isWindows)
      //windows automatic .exe start on startup register
      SaveAutoStartWindows();
    else if (isLinux)
      SaveAutoStartLinux();
    else
      Log.Error("Failed to add application to autostart, unsupported OS");
  }

  private static void SaveAutoStartWindows() {
    try {
      using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);
      if (key != null) {
        var appName = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetEntryAssembly().Location);
        key.SetValue(appName, System.Reflection.Assembly.GetEntryAssembly().Location);
      }

      Log.Information("Application added to Windows autostart");
    }
    catch (Exception ex) {
      Log.Error("Failed to add application to Windows autostart");
    }
  }

  private static void SaveAutoStartLinux() {
    try {
      var desktopFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.config/autostart/myapp.desktop";

      // Writing to the .desktop file
      using var writer = new StreamWriter(desktopFilePath);
      writer.WriteLine("[Desktop Entry]");
      writer.WriteLine("Type=Application");
      writer.WriteLine("Name=My Application");
      writer.WriteLine("Exec=/path/to/your/application/executable");
      writer.WriteLine("X-GNOME-Autostart-enabled=true");

      Log.Information("Application added to Linux autostart");
    }
    catch (Exception ex) {
      Log.Error("Failed to add application to Linux autostart");
    }
  }
}