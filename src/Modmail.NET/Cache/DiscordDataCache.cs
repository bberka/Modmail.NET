using Modmail.NET.Abstract;

namespace Modmail.NET.Cache;

public class DiscordDataCache : BaseCache
{
  private static DiscordDataCache? _instance;
  private DiscordDataCache() { }

  public static DiscordDataCache This {
    get {
      _instance ??= new DiscordDataCache();
      return _instance;
    }
  }
}