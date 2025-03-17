using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Modmail.NET.Models.Dto;

public sealed record DiscordTicketMessageDto(DiscordClient Sender, MessageCreateEventArgs Args);