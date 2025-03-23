namespace Modmail.NET.Exceptions;

public class NotJoinedMainServerException : BotExceptionBase
{
  public NotJoinedMainServerException() : base(LangProvider.This.GetTranslation(LangKeys.NOT_JOINED_MAIN_SERVER)) { }
}