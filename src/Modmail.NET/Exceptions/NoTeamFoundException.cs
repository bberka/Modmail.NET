namespace Modmail.NET.Exceptions;

public class NoTeamFoundException : BotExceptionBase
{
  public NoTeamFoundException() : base(LangData.This.GetTranslation(LangKeys.NO_TEAM_FOUND)) { }
}