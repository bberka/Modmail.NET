namespace Modmail.NET.Cache;

public class DatabaseCache : BaseCache
{
    private static DatabaseCache? _instance;

    private DatabaseCache()
    {
    }

    public static DatabaseCache This
    {
        get
        {
            _instance ??= new DatabaseCache();
            return _instance;
        }
    }
}