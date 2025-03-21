namespace Modmail.NET.Exceptions;

public class DbInternalException : BotExceptionBase
{
  public DbInternalException() : base(LangProvider.This.GetTranslation(LangKeys.DB_INTERNAL_ERROR)) { }
}