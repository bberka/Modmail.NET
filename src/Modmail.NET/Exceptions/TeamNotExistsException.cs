namespace Modmail.NET.Exceptions;

public class TeamNotExistsException : BotExceptionBase
{
  public TeamNotExistsException() : base(LangProvider.This.GetTranslation(LangKeys.TEAM_NOT_EXISTS)) { }
}