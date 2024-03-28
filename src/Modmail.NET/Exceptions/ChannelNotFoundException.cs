using Modmail.NET.Static;

namespace Modmail.NET.Exceptions;

public class ChannelNotFoundException : BotExceptionBase
{
  public ChannelNotFoundException(ulong id) : base(Texts.CHANNEL_NOT_FOUND) {
    Id = id;
  }

  public ulong Id { get; }
}