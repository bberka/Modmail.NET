using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class DbInternalException : BotExceptionBase
{
  public DbInternalException() : base(LangProvider.This.GetTranslation(LangKeys.DbInternalError)) { }
}