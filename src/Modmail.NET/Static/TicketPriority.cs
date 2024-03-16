using DSharpPlus.SlashCommands;

namespace Modmail.NET.Static;

public enum TicketPriority
{
  
  [ChoiceName("Low")]
  Low,
  [ChoiceName("Normal")]
  Normal,
  [ChoiceName("High")]
  High
}