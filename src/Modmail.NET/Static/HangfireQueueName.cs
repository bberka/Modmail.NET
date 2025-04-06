using Ardalis.SmartEnum;

namespace Modmail.NET.Static;

/// <summary>
///   Hangfire queue names must be lower case
/// </summary>
public sealed class HangfireQueueName : SmartEnum<HangfireQueueName>
{
  public static readonly HangfireQueueName Default = new("default", 0); //do not change this
  private HangfireQueueName(string name, int value) : base(name, value) { }
}