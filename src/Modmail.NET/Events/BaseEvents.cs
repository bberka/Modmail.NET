using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace Modmail.NET.Events;

public static class BaseEvents
{
  public static async Task OnHeartbeat(DiscordClient sender, HeartbeatEventArgs args) {
    Log.Verbose("Heartbeat received from {Username}", sender.CurrentUser.Username);
  }

  public static async Task OnReady(DiscordClient sender, ReadyEventArgs args) {
    Log.Information("Client is ready to process events");
  }

  public static async Task OnClientError(DiscordClient sender, ClientErrorEventArgs args) {
    Log.Error(args.Exception, "Exception occured in {Client}", sender.CurrentUser.Username);
  }

  public static async Task OnSocketError(DiscordClient sender, SocketErrorEventArgs args) {
    Log.Error(args.Exception, "Socket error occured in {Client}", sender.CurrentUser.Username);
  }
}