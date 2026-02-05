using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;

namespace Modmail.NET.Features.Ticket.Static;

public enum TicketPriority
{
    [ChoiceDisplayName("Low")]
    Low,

    [ChoiceDisplayName("Normal")]
    Normal,

    [ChoiceDisplayName("High")]
    High
}