using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace Modmail.NET.Events;

public class OnClientError
{
  public static async Task Handle(DiscordClient sender, ClientErrorEventArgs args) {
    Log.Error(args.Exception, "Exception occured in {Client}", sender.CurrentUser.Username);
  }
}