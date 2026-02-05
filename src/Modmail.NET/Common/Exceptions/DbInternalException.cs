using Modmail.NET.Language;

namespace Modmail.NET.Common.Exceptions;

public class DbInternalException : ModmailBotException
{
  public DbInternalException() : base(LangProvider.This.GetTranslation(LangKeys.DbInternalError)) { }
}