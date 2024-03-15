using Modmail.NET.Abstract;

namespace Modmail.NET.Cache;

public class DatabaseCache : BaseCache
{
  private DatabaseCache() { }

  public static DatabaseCache This {
    get {
      _instance ??= new();
      return _instance;
    }
  }

  private static DatabaseCache? _instance;
  
}