using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;

namespace Modmail.NET;

public class BotConfig
{
  public EnvironmentType Environment { get; set; }
  public required string BotToken { get; set; }
  public required string BotClientId { get; set; }
  public required string BotClientSecret { get; set; }
  public required string BotPrefix { get; set; }
  public ulong MainServerId { get; set; }
  public required ulong[] SuperUsers { get; set; }
  public required string DbConnectionString { get; set; }
  public required string DefaultLanguage { get; set; }
  public bool SensitiveEfCoreDataLog { get; set; }

  [Url]
  public required string Domain { get; set; }
}