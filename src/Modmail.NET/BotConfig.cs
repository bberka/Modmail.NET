using System.ComponentModel.DataAnnotations;
using Modmail.NET.Common.Static;

namespace Modmail.NET;

public class BotConfig
{
  public EnvironmentType Environment { get; set; }
  public string BotToken { get; set; }
  public string BotClientId { get; set; }
  public string BotClientSecret { get; set; }
  public string BotPrefix { get; set; }
  public ulong MainServerId { get; set; }
  public ulong[] OwnerUsers { get; set; }
  public string DbConnectionString { get; set; }
  public string DefaultLanguage { get; set; }
  public bool SensitiveEfCoreDataLog { get; set; }

  [Url]
  public string Domain { get; set; }
}