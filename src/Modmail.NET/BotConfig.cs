using Microsoft.Extensions.Configuration;

namespace Modmail.NET;

public class BotConfig
{
    private BotConfig()
    {
        var configManager = new ConfigurationBuilder().AddJsonFile("appsettings.json", false, true)
            .AddJsonFile("appsettings.Development.json", true, true)
            .Build();

        Environment = Enum.Parse<EnvironmentType>(configManager["Bot:Environment"] ?? "Development", true);
        BotToken = configManager["Bot:BotToken"] ?? throw new Exception("Bot:BotToken is not set.");
        BotClientId = configManager["Bot:BotClientId"] ?? throw new Exception("Bot:BotClientId is not set.");
        BotClientSecret = configManager["Bot:BotClientSecret"] ?? throw new Exception("Bot:BotClientSecret is not set.");
        BotPrefix = configManager["Bot:BotPrefix"] ?? "!!";
        MainServerId = ulong.Parse(configManager["Bot:MainServerId"] ?? "0");
        SuperUsers = configManager["Bot:SuperUsers"]
            ?.Split(',')
            .Select(ulong.Parse)
            .ToArray() ?? [];
        DbConnectionString = configManager["Bot:DbConnectionString"] ?? throw new Exception("Bot:DbConnectionString is not set.");
    }

    public static BotConfig This
    {
        get
        {
            field ??= new BotConfig();
            return field;
        }
    }

    public EnvironmentType Environment { get; set; }
    public string BotToken { get; set; }
    public string BotClientId { get; set; }
    public string BotClientSecret { get; set; }
    public string BotPrefix { get; set; }
    public ulong MainServerId { get; set; }
    public ulong[] SuperUsers { get; set; }
    public string DbConnectionString { get; set; }
}