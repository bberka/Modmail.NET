using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class MemberAlreadyInTeamException : ModmailBotException
{
  public MemberAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.MemberAlreadyInTeam)) { }
}