using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class MainGuildNotFoundException : BotExceptionBase
{
  public MainGuildNotFoundException() : base(Texts.MAIN_GUILD_NOT_FOUND, Texts.MAIN_GUILD_NOT_FOUND_DESC) { }
}