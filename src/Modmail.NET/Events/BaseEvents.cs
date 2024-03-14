using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Modmail.NET.Events;

public static class BaseEvents
{
  public static async Task OnHeartbeat(DiscordClient sender, HeartbeatEventArgs args) {
  }

  public static async Task OnReady(DiscordClient sender, ReadyEventArgs args) {
  }

  public static async Task OnClientError(DiscordClient sender, ClientErrorEventArgs args) {
  }

  public static async Task OnSocketError(DiscordClient sender, SocketErrorEventArgs args) {
  }
}