using Modmail.NET.Abstract;

namespace Modmail.NET.Exceptions;

public class NotJoinedMainServerException : BotExceptionBase
{
  public NotJoinedMainServerException() : base(LangProvider.This.GetTranslation(LangKeys.NotJoinedMainServer)) { }
}