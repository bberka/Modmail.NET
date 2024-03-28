using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TeamAlreadyExistsException : BotExceptionBase
{
  public TeamAlreadyExistsException() : base(Texts.TEAM_ALREADY_EXISTS) { }
}