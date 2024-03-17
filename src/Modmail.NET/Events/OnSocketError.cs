using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace Modmail.NET.Events;

public class OnSocketError
{
  public static async Task Handle(DiscordClient sender, SocketErrorEventArgs args) {
    Log.Error(args.Exception, "Socket error occured in {Client}", sender.CurrentUser.Username);
  }
}