using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class LogChannelNotFoundException : BotExceptionBase
{
  public LogChannelNotFoundException() : base(Texts.LOG_CHANNEL_NOT_FOUND) { }
}