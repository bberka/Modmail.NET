namespace Modmail.NET.Exceptions;

public class MainGuildNotFoundException : BotExceptionBase
{
  public MainGuildNotFoundException() : base(LangData.This.GetTranslation(LangKeys.MAIN_GUILD_NOT_FOUND), LangData.This.GetTranslation(LangKeys.MAIN_GUILD_NOT_FOUND_DESC)) { }
}