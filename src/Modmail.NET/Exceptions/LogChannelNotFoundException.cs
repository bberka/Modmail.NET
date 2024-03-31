namespace Modmail.NET.Exceptions;

public class LogChannelNotFoundException : BotExceptionBase
{
  public LogChannelNotFoundException() : base(LangData.This.GetTranslation(LangKeys.LOG_CHANNEL_NOT_FOUND)) { }
}