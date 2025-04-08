using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class RoleAlreadyInTeamException : ModmailBotException
{
  public RoleAlreadyInTeamException() : base(LangProvider.This.GetTranslation(LangKeys.RoleAlreadyInTeam)) { }
}