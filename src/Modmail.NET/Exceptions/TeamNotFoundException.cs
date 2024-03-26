using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class TeamNotFoundException : BotExceptionBase
{
  public TeamNotFoundException() : base(Texts.TEAM_NOT_FOUND) { }
}