using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using Modmail.NET.Features.Ticket.Services;

namespace Modmail.NET.Features.DiscordBot.Events;

public static class OnMessageCreatedEvent
{
  public static async Task OnMessageCreated(DiscordClient client, MessageCreatedEventArgs args) {
    _ = UserUpdateEvents.UpdateUser(client, args.Author);
    if (args.Message.Author?.IsBot == true) return;
    if (args.Message.IsTTS) return;

    var scope = client.ServiceProvider.CreateScope();
    var ticketMessageQueue = scope.ServiceProvider.GetRequiredService<TicketMessage>();
    await ticketMessageQueue.Enqueue(args.Author.Id, args);
  }
}