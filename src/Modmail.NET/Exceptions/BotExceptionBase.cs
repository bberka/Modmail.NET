namespace Modmail.NET.Exceptions;

public abstract class BotExceptionBase : Exception
{
  protected BotExceptionBase(string titleMessage, string? contentMessage = null) {
    TitleMessage = titleMessage;
    ContentMessage = contentMessage;
  }

  public string TitleMessage { get; }
  public string? ContentMessage { get; }
}