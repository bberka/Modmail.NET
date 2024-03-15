﻿using Microsoft.Extensions.Configuration;
using Modmail.NET.Static;
using Serilog.Events;

namespace Modmail.NET.Common;

public class MMConfig
{
  private static MMConfig? _instance;
  private readonly IConfigurationRoot _configManager;

  private MMConfig() {
    _configManager = new ConfigurationBuilder()
                     .AddJsonFile("appsettings.json", false, true)
                     .Build();
    Environment = Enum.Parse<EnvironmentType>(_configManager["ENVIRONMENT"] ?? "Development", true);
    BotToken = _configManager["BOT_TOKEN"] ?? throw new Exception("BOT_TOKEN is not set.");
    BotClientId = _configManager["BOT_CLIENT_ID"] ?? throw new Exception("BOT_CLIENT_ID is not set.");
    BotClientSecret = _configManager["BOT_CLIENT_SECRET"] ?? throw new Exception("BOT_CLIENT_SECRET is not set.");
    BotPrefix = _configManager["BOT_PREFIX"] ?? "!";
    MainServerId = ulong.Parse(_configManager["MAIN_SERVER_ID"] ?? "0");
    OwnerUsers = _configManager["OWNER_USERS"]?.Split(',').Select(ulong.Parse).ToArray() ?? Array.Empty<ulong>();
    DbType = Enum.Parse<DbType>(_configManager["DB_TYPE"] ?? "Sqlite", true);
    DbConnectionString = _configManager["DB_CONNECTION_STRING"] ?? throw new Exception("DB_CONNECTION_STRING is not set.");
    LogLevel = Enum.Parse<LogEventLevel>(_configManager["LOG_LEVEL"] ?? "Information", true);
    LogSinkToFile = bool.Parse(_configManager["LOG_SINK_TO_FILE"] ?? "true");
    LogSinkToConsole = bool.Parse(_configManager["LOG_SINK_TO_CONSOLE"] ?? "true");
  }

  public static MMConfig This {
    get {
      _instance ??= new MMConfig();
      return _instance;
    }
  }

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
  public ulong MainServerId { get; set; }

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