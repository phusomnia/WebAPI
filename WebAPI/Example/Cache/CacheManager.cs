using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Annotation;

namespace WebAPI.Example.Cache;

[Component]
public class CacheManager
{
    private readonly IMemoryCache _memoryCache;
    private readonly ConcurrentDictionary<string, bool> _cacheKeys;

    public CacheManager(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheKeys = new ConcurrentDictionary<string, bool>();
    }

    public void Set<T>(string key, T value, MemoryCacheEntryOptions options)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty");
        }
        _memoryCache.Set(key, value, options);
        _cacheKeys.TryAdd(key, true);
    }

    public bool tryGetValue<T>(string key, out T? value)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        if (_memoryCache.TryGetValue(key, out value))
        {
            return true;
        }

        _cacheKeys.TryRemove(key, out _);
        value = default;
        return false;
    }

    public void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            throw new ArgumentException("Key cannot be null or empty.", nameof(key));
        }

        _memoryCache.Remove(key);
        _cacheKeys.TryRemove(key, out _);
    }

    public List<String> getAllKeys()
    {
        return _cacheKeys.Keys.ToList();
    }

    public void Clear()
    {
        foreach (var key in _cacheKeys)
        {
            _memoryCache.Remove(key);
        }

        _cacheKeys.Clear();
    }
}
