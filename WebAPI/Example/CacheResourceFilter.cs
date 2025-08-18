using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Example;

public class CacheResourceFilter : Attribute, IResourceFilter
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromSeconds(10);

    public CacheResourceFilter(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var key = GenerateCacheKey(context);
        if (_memoryCache.TryGetValue(key, out var cachedValue))
        {
            Console.WriteLine($"Cache hit for key: {key}");
            context.Result = cachedValue as IActionResult;
        }
        else
        {
            Console.WriteLine($"Cache miss for key: {key}");
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context)
    {
        if (context.Result is ObjectResult objectResult)
        {
            var cacheKey = GenerateCacheKey(context);
            _memoryCache.Set(cacheKey, objectResult, _cacheDuration);
            Console.WriteLine($"Cache set for key: {cacheKey}");
        }
    }

    private string GenerateCacheKey(FilterContext context)
    {
        return $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
    }
}
