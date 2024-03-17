using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace Modmail.NET.Events;

public static class OnHeartbeat
{
   public static async Task Handle(DiscordClient sender, HeartbeatEventArgs args) {
      Log.Verbose("Heartbeat received from {Username}", sender.CurrentUser.Username);
   }

}