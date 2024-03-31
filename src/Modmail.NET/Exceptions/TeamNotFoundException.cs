namespace Modmail.NET.Exceptions;

public class TeamNotFoundException : BotExceptionBase
{
  public TeamNotFoundException() : base(LangData.This.GetTranslation(LangKeys.TEAM_NOT_FOUND)) { }
}