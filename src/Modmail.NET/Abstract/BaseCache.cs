using System.Collections.Concurrent;

namespace Modmail.NET.Abstract;

public abstract class BaseCache
{
    protected readonly ConcurrentDictionary<string, CacheData> CacheDictionary;

    protected BaseCache()
    {
        CacheDictionary = new ConcurrentDictionary<string, CacheData>();

        Task.Run(() =>
        {
            while (true)
            {
                var expiredKeys = CacheDictionary.Where(x => x.Value.ExpireDate < DateTime.Now)
                    .Select(x => x.Key)
                    .ToList();
                foreach (var key in expiredKeys) CacheDictionary.TryRemove(key, out _);

                Thread.Sleep(1000);
            }
        });
    }

    public object? Get(string key)
    {
        return CacheDictionary.TryGetValue(key, out var cacheData) ? cacheData.Value : null;
    }

    public string GetString(string key)
    {
        var value = Get(key);
        return value?.ToString() ?? string.Empty;
    }

    public void Set(
        string key,
        object value,
        TimeSpan expireTimeSpan
    )
    {
        CacheDictionary[key] = new CacheData(value, DateTime.Now.Add(expireTimeSpan));
    }

    protected record CacheData(object Value, DateTime ExpireDate);
}