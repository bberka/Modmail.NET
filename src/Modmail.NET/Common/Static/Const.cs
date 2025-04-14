using DSharpPlus.Entities;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Static;

public static class Const
{
  public const string ThemeCookieName = "Modmail.NET.Theme";
  public static readonly DiscordActivity DiscordActivity = new(LangProvider.This.GetTranslation(Lang.ModerationConcerns), DiscordActivityType.ListeningTo);
}