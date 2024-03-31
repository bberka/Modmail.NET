using Modmail.NET.Language;

namespace Modmail.NET.Exceptions;

public class ChannelNotFoundException : BotExceptionBase
{
  public ChannelNotFoundException(ulong id) : base(LangData.This.GetTranslation(LangKeys.CHANNEL_NOT_FOUND)) {
    Id = id;
  }

  public ulong Id { get; }
}