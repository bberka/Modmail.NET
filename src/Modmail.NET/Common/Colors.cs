﻿using DSharpPlus.Entities;

namespace Modmail.NET.Common;

public static class Colors
{
  public static readonly DiscordColor ErrorColor = DiscordColor.DarkRed;
  public static readonly DiscordColor SuccessColor = DiscordColor.Green;
  public static readonly DiscordColor InfoColor = DiscordColor.CornflowerBlue;

  public static readonly DiscordColor WarningColor = DiscordColor.Orange;
  // private static readonly DiscordColor NeutralInformationColor = DiscordColor.Gold;


  public static readonly DiscordColor MessageReceivedColor = DiscordColor.Blue;
  public static readonly DiscordColor MessageSentColor = DiscordColor.Green;
  public static readonly DiscordColor TicketCreatedColor = DiscordColor.Blue;
  public static readonly DiscordColor TicketClosedColor = DiscordColor.Red;
  public static readonly DiscordColor TicketPriorityChangedColor = DiscordColor.Magenta;
  public static readonly DiscordColor TicketTypeChangedColor = DiscordColor.SpringGreen;
  public static readonly DiscordColor NoteAddedColor = DiscordColor.Teal;
  public static readonly DiscordColor AnonymousToggledColor = DiscordColor.Grayple;
  public static readonly DiscordColor FeedbackColor = DiscordColor.Orange;
}