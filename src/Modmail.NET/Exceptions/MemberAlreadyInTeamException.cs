using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class MemberAlreadyInTeamException : BotExceptionBase
{
  public MemberAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.MemberAlreadyInTeam)) { }
}