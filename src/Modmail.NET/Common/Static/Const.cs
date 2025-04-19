using DSharpPlus.Entities;
using Modmail.NET.Language;

namespace Modmail.NET.Common.Static;

public static class Const
{
	public const string ThemeCookieName = "Modmail.NET.Theme";
	public const string UrlRegex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";
	public static readonly DiscordActivity DiscordActivity = new(Lang.ModerationConcerns.Translate(), DiscordActivityType.ListeningTo);
}