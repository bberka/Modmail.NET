using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Modmail.NET.Static;

public enum TicketPriority
{
  [ChoiceDisplayName("Low")]
  Low,

  [ChoiceDisplayName("Normal")]
  Normal,

  [ChoiceDisplayName("High")]
  High
}