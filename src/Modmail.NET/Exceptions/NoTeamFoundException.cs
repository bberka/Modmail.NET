using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class NoTeamFoundException : BotExceptionBase
{
  public NoTeamFoundException() : base(Texts.NO_TEAM_FOUND) { }
}