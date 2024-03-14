using Modmail.NET.Abstract;

namespace Modmail.NET.Common;

public static class DotEnv
{
  /// <summary>
  ///  Initializes the environment variables from the .env file.
  /// </summary>
  /// <exception cref="FileNotFoundException"></exception>
  public static void Init() {
    var workingDirectory = Environment.CurrentDirectory;
    var projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName;
    var envPath = string.IsNullOrEmpty(projectDirectory)
                    ? Path.Combine(workingDirectory, ".env")
                    : Path.Combine(projectDirectory, ".env");
    if (!File.Exists(envPath)) {
      throw new FileNotFoundException("The .env file was not found in the current directory.");
    }

    var lines = File.ReadAllLines(envPath);
    foreach (var line in lines) {
      if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line) || !line.Contains('='))
        continue;


      var parts = line.Trim()
                      .Split(
                             '=',
                             StringSplitOptions.RemoveEmptyEntries);

      var merged = string.Join('=', parts[1..]);
      Environment.SetEnvironmentVariable(parts[0].Trim(),merged.Trim('"').Trim());
    }
  }
}