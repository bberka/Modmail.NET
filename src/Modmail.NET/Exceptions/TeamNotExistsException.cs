using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class TeamNotExistsException : BotExceptionBase
{
  public TeamNotExistsException() : base(LangProvider.This.GetTranslation(LangKeys.TeamNotExists)) { }
}