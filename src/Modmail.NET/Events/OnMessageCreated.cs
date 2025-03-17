using DSharpPlus;
using DSharpPlus.EventArgs;
using Metran;
using Modmail.NET.Entities;
using Modmail.NET.Models.Dto;
using Modmail.NET.Queues;

namespace Modmail.NET.Events;

public static class OnMessageCreated
{
  private static readonly MetranContainer<ulong> ProcessingUserMessageContainer = new();

  public static async Task Handle(DiscordClient sender, MessageCreateEventArgs args) {
    await DiscordUserInfo.AddOrUpdateAsync(args.Author);
    if (args.Message.Author.IsBot) return;
    if (args.Message.IsTTS) return;
    await TicketMessageQueue.This.EnqueueMessage(args.Author.Id, new DiscordTicketMessageDto(sender, args));
  }
}