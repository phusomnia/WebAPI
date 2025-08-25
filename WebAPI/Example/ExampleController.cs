using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.Example.Cache;
using WebAPI.Example.Validate;

namespace WebAPI.Example;

[ApiController]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly CacheManager _cache;

    public ExampleController(
        ILogger<ExampleController> logger,
        CacheManager cache
    )
    {
        _logger = logger;
        _cache = cache;
    }

    [HttpGet("/middleware")]
    [TypeFilter(typeof(CustomActionFilter))]
    public IActionResult Get(
        [FromQuery] string? name,
        [FromHeader] string? header
    )
    {
        foreach (var headerItem in Request.Headers)
        {
            Console.WriteLine("Request Header: {0} = {1}", headerItem.Key, headerItem.Value);
        }
        _logger.LogInformation("Request Header: {0} = {1}", header, header);
        return Ok("Hello " + name);
    }

    [HttpGet("/cache")]
    [TypeFilter(typeof(CacheResourceFilter))]
    public IActionResult Cache()
    {
        var data = GetDataFromListAsync();
        return Ok(data);
    }

    [HttpGet("/error")]
    [TypeFilter(typeof(CustomExceptionFilter))]
    public Task<IActionResult> Error()
    {
        _logger.LogError("System error occurred");
        throw new InvalidOperationException("System error occurred");
    }

    [HttpPost("/inmem-cache")]
    public IActionResult setCache([FromBody] CacheItem item)
    {
        var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(30));
        _cache.Set<object>(item.Key, item.Value, options);
        return Ok(item);
    }

    [HttpGet("/inmem-cache/{key}")]
    public IActionResult getCache(string key)
    {
        if (!_cache.tryGetValue<object>(key, out var value))
        {
            return NotFound();
        }
        return Ok(value);
    }

    [HttpGet("/pub-sub")]
    public IActionResult pubsub()
    {
        return Ok("pub sub");
    }

    [HttpGet("/exception")]
    public IActionResult handleException()
    {
        throw new Exception("Ex");
    }

    [HttpPost("/validate")]
    public IActionResult validateDemo(ValidateDTO dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        
        return Ok("Ok");
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

public class CacheItem
{
    public String Key { get; set; }
    public Object Value { get; set; }

    public override string ToString()
    {
        return $"Key: {Key} and value: {Value}";
    }
}
