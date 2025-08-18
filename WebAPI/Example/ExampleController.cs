using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI.Example;

[ApiController]
public class ExampleController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly ExecutionTimeLogger _executionTimeLogger;
    private readonly CacheResourceFilter _cacheResourceFilter;

    public ExampleController(
        IMemoryCache memoryCache,
        ExecutionTimeLogger executionTimeLogger,
        CacheResourceFilter cacheResourceFilter
        )
    {
        _memoryCache = memoryCache;
        _executionTimeLogger = executionTimeLogger;
        _cacheResourceFilter = cacheResourceFilter;
    }

    [HttpGet("/middleware")]
    [ServiceFilter(typeof(ExecutionTimeLogger))]
    public async Task<dynamic> Get()
    {
        await Task.Delay(1000);
        return "Hello World!";
    }
    
    [HttpGet("/cache")]
    [ServiceFilter(typeof(CacheResourceFilter))]
    public IActionResult Cache()
    {
        var data = GetDataFromListAsync();
        return Ok(data);
    }

    private object GetDataFromListAsync()
    {
        return new List<string>
        {
            "Item 1", "Item 2", "Item 3"
        };
    }
}

class CacheEntryMetadata<T>
{
    public T? Data { get; set; }
    public DateTime ExpirationTime { get; set; }
}