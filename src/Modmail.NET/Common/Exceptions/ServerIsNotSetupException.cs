using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class ServerIsNotSetupException : ModmailBotException
{
  public ServerIsNotSetupException() : base(LangProvider.This.GetTranslation(LangKeys.RoleNotFoundInTeam)) { }
}