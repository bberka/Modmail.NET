using System.Collections.Concurrent;

namespace Modmail.NET.Common;

public sealed class SimpleCacher
{
    private static readonly Lazy<SimpleCacher> _instance = new(() => new SimpleCacher());
    private readonly ConcurrentDictionary<string, CacheItem> _cache;

    private SimpleCacher()
    {
        _cache = new ConcurrentDictionary<string, CacheItem>();
    }

    public static SimpleCacher Instance => _instance.Value;

    // Add an item to the cache with expiration
    public void Set<T>(
        string key,
        T value,
        TimeSpan expiration
    )
    {
        if (value is null) throw new ArgumentNullException(nameof(value), "Value cannot be null.");

        var cacheItem = new CacheItem(value, expiration);
        if (!_cache.TryAdd(key, cacheItem)) _cache[key] = cacheItem;
    }

    // Retrieve an item from the cache
    public T Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (!cacheItem.IsExpired) return (T)cacheItem.Value;

            // Remove expired item
            Remove(key);
        }

        throw new KeyNotFoundException("Key not found in the cache.");
    }

    // Remove an item from the cache
    public bool Remove(string key)
    {
        return _cache.TryRemove(key, out _);
    }

    // Clear the cache
    public void Clear()
    {
        _cache.Clear();
    }

    // Check if the cache contains a key
    public bool ContainsKey(string key)
    {
        return _cache.TryGetValue(key, out var cacheItem) && !cacheItem.IsExpired;
    }

    // Get or set an item in the cache
    public async Task<T?> GetOrSetAsync<T>(
        string key,
        Func<Task<T>> func,
        TimeSpan expiration
    )
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (!cacheItem.IsExpired) return (T)cacheItem.Value;

            Remove(key); // Remove expired item
        }

        var result = await func();
        if (result is null) return result;
        // throw new ArgumentNullException(nameof(result), "Value cannot be null.");
        Set(key, result, expiration);
        return result;
    }

    public T GetOrSet<T>(
        string key,
        Func<Task<T>> func,
        TimeSpan expiration
    )
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (!cacheItem.IsExpired) return (T)cacheItem.Value;

            Remove(key); // Remove expired item
        }

        var result = func()
            .GetAwaiter()
            .GetResult();
        Set(key, result, expiration);
        return result;
    }

    // Create a unique key for caching based on method and parameters
    public static string CreateKey(
        string classKey,
        string methodKey,
        object? parameters = null
    )
    {
        return $"{classKey}.{methodKey}({parameters})";
    }

    private class CacheItem
    {
        public CacheItem(object value, TimeSpan expiration)
        {
            Value = value;
            Expiration = DateTime.UtcNow.Add(expiration);
        }

        public object Value { get; }
        public DateTime Expiration { get; }

        public bool IsExpired => DateTime.UtcNow > Expiration;
    }
}