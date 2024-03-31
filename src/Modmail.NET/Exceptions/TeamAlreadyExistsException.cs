namespace Modmail.NET.Exceptions;

public class TeamAlreadyExistsException : BotExceptionBase
{
  public TeamAlreadyExistsException() : base(LangData.This.GetTranslation(LangKeys.TEAM_ALREADY_EXISTS)) { }
}