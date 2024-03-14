using Modmail.NET.Static;
using Serilog.Events;

namespace Modmail.NET.Common;

public class EnvContainer
{
  public bool IsInitialized { get; private set; } = false;

  private EnvContainer() {
    Environment = Enum.Parse<EnvironmentType>(System.Environment.GetEnvironmentVariable("ENVIRONMENT") ?? "Development", true);
    BotToken = System.Environment.GetEnvironmentVariable("BOT_TOKEN") ?? throw new Exception("BOT_TOKEN is not set.");
    BotClientId = System.Environment.GetEnvironmentVariable("BOT_CLIENT_ID") ?? throw new Exception("BOT_CLIENT_ID is not set.");
    BotClientSecret = System.Environment.GetEnvironmentVariable("BOT_CLIENT_SECRET") ?? throw new Exception("BOT_CLIENT_SECRET is not set.");
    BotPrefix = System.Environment.GetEnvironmentVariable("BOT_PREFIX") ?? "!";
    MainServerId = long.Parse(System.Environment.GetEnvironmentVariable("MAIN_SERVER_ID") ?? "0");
    OwnerUsers = System.Environment.GetEnvironmentVariable("OWNER_USERS")?.Split(',').Select(ulong.Parse).ToArray() ?? Array.Empty<ulong>();
    DbType = Enum.Parse<DbType>(System.Environment.GetEnvironmentVariable("DB_TYPE") ?? "Sqlite", true);
    DbConnectionString = System.Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ?? throw new Exception("DB_CONNECTION_STRING is not set.");
    LogLevel = Enum.Parse<LogEventLevel>(System.Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "Information", true);
    LogSinkToFile = bool.Parse(System.Environment.GetEnvironmentVariable("LOG_SINK_TO_FILE") ?? "true");
    LogSinkToConsole = bool.Parse(System.Environment.GetEnvironmentVariable("LOG_SINK_TO_CONSOLE") ?? "true");

    IsInitialized = true;
  }

  public static EnvContainer This {
    get {
      _instance ??= new();
      return _instance;
    }
  }

  private static EnvContainer? _instance;

  //Name: ENVIRONMENT
  public EnvironmentType Environment { get; set; }

  //Name: BOT_TOKEN
  public string BotToken { get; set; }

  //Name: BOT_CLIENT_ID
  public string BotClientId { get; set; }

  //Name: BOT_CLIENT_SECRET
  public string BotClientSecret { get; set; }

  //Name: BOT_PREFIX
  public string BotPrefix { get; set; }

  //Name: MAIN_SERVER_ID
  public long MainServerId { get; set; }

  //Name: OWNER_USERS
  public ulong[] OwnerUsers { get; set; }


  //Name: DB_TYPE
  public DbType DbType { get; set; }

  //Name: DB_CONNECTION_STRING
  public string DbConnectionString { get; set; }

  //Name: ENCRYPTION_KEY
  public string EncryptionKey { get; set; }


  //Name:  LOG_LEVEL
  public LogEventLevel LogLevel { get; set; }

  //Name: LOG_SINK_TO_FILE
  public bool LogSinkToFile { get; set; }

  //Name: LOG_SINK_TO_CONSOLE
  public bool LogSinkToConsole { get; set; }
}