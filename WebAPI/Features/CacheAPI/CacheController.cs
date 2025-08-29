using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using WebAPI.Features.Cache;
using WebAPI.Features.CacheAPI.Redis;

[ApiController]
[Route("/api/v1/")]
public class CacheController : ControllerBase
{
    private readonly RedisConfig _redisConfig;
    private readonly ICacheBase _redisStorage;

    public CacheController(RedisConfig redisConfig, ICacheBase redisStorage)
    {
        _redisConfig = redisConfig;
        _redisStorage = redisStorage;
    }

    [HttpGet("cache/test")]
    public IActionResult test()
    {
        return Ok(_redisStorage.test());
    }

    [HttpGet("cacheConfig")]
    public IActionResult checkConfig()
    {
        Console.WriteLine("Config {0}", _redisConfig.redis().Multiplexer.Configuration);
        return Ok("Ok");
    }

    [HttpPost("cache")]
    public IActionResult setValue(RedisDTO req)
    {
        return Ok(_redisStorage.set(req.Key, req.Value));
    }

    [HttpGet("cache")]
    public IActionResult getValue(string key)
    {
        return Ok(_redisStorage.get(key));
    }
}

public class RedisDTO
{
    public string Key { get; set; }
    public object Value { get; set; }
}
