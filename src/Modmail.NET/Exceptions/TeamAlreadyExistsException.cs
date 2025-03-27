using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TeamAlreadyExistsException : BotExceptionBase
{
  public TeamAlreadyExistsException() : base(LangProvider.This.GetTranslation(LangKeys.TeamAlreadyExists)) { }
}