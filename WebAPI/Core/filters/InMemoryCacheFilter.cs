using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Core.filters;

public class InMemoryCacheFilter : IAsyncResourceFilter
{
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheDuration;
    private readonly ILogger<IMemoryCache> _logger;

    public InMemoryCacheFilter(
        IConfiguration config,
        IMemoryCache memoryCache,
        ILogger<IMemoryCache> logger
    )
    {
        var duration = new DataTable().Compute(config["InMemoryCache:cacheDuration"], null).ToString();
        _memoryCache = memoryCache;
        _cacheDuration = TimeSpan.FromMilliseconds(Convert.ToInt32(duration));
        _logger = logger;
    }

    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {   
        var cacheKey = GenerateCacheKey(context).Split("/")[1];
        _logger.LogInformation($"{nameof(cacheKey)}: {cacheKey}\n");
        
        if (_memoryCache.TryGetValue(cacheKey, out Object cachedValue))
        {
            _logger.LogInformation($"Cache hit for key: {cacheKey}\n");
            context.Result = cachedValue as IActionResult;
            return;
        }

        _logger.LogInformation($"Cache miss for key: {cacheKey}\n");
        var executedContext = await next();
        
        if (executedContext.Result is ObjectResult objectResult)
        {
            _logger.LogInformation($"Caching response for key: {cacheKey}\n");
            _memoryCache.Set(cacheKey, objectResult, _cacheDuration);
        }
    }

    private string GenerateCacheKey(ResourceExecutingContext context)
    {
        return $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}";
    }
}