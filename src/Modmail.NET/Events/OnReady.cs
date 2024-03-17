﻿using DSharpPlus;
using DSharpPlus.EventArgs;
using Serilog;

namespace Modmail.NET.Events;

public class OnReady
{
  public static async Task Handle(DiscordClient sender, ReadyEventArgs args) {
    Log.Information("Client is ready to process events");
  }

}