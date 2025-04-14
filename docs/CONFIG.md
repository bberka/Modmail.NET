# Configuration

# Bot Section
- `Environment`: The environment in which the bot is running. Common values are `development` and `production`.
- `BotToken`: The bot token used to authenticate with Discord.
- `BotClientId`: The client ID of the bot application.
- `BotClientSecret`: The client secret of the bot application.
- `BotPrefix`: The prefix used for bot commands.
- `MainServerId`: The ID of the main server where the bot is primarily used.
- `SuperUsers`: A list of user IDs that have superuser privileges.
- `DbConnectionString`: The connection string for the database.
- `DefaultLanguage`: The default language for the bot.
- `SensitiveEfCoreDataLog`: A boolean indicating whether to log sensitive data in Entity Framework Core.
- `Domain`: The domain used for the bot.

# Logger Sections
- `AppLogger`: Configuration for the application logger.
- `AspNetCoreLogger`: Configuration for the ASP.NET Core logger.
- `EfCoreLogger`: Configuration for the Entity Framework Core logger.

All loggers use Serilog for logging configuration.

## Full Configuration Example

```json
{
  "Bot": {
    "Environment": "development",
    "BotToken": "super_secret_bot_token",
    "BotClientId": "bot_client_id",
    "BotClientSecret": "bot_client_secret",
    "BotPrefix": "!!",
    "MainServerId": 0,
    "SuperUsers": [
      0
    ],
    "DbConnectionString": "Server=localhost; Database=Modmail_NET;",
    "DefaultLanguage": "en",
    "SensitiveEfCoreDataLog": false,
    "Domain": "https://"
  },
  "AppLogger": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": "Information",
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithProcessId",
      "WithThreadId"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/log-.json",
          "rollingInterval": "Day",
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  },
  "AspNetCoreLogger": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": "Warning",
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithProcessId",
      "WithThreadId"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/asp-log-.json",
          "rollingInterval": "Day",
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  },
  "EfCoreLogger": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File"
    ],
    "MinimumLevel": "Warning",
    "Enrich": [
      "FromLogContext",
      "WithMachineName",
      "WithProcessId",
      "WithThreadId"
    ],
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/ef-log-.json",
          "rollingInterval": "Day",
          "rollOnFileSizeLimit": true,
          "formatter": "Serilog.Formatting.Compact.CompactJsonFormatter, Serilog.Formatting.Compact"
        }
      }
    ]
  }
}
```

