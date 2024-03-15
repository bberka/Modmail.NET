using Modmail.NET.Abstract;

namespace Modmail.NET.Cache;

public class DiscordDataCache : BaseCache
{
  private DiscordDataCache() { }

  public static DiscordDataCache This {
    get {
      _instance ??= new();
      return _instance;
    }
  }

  private static DiscordDataCache? _instance;

  
  
}