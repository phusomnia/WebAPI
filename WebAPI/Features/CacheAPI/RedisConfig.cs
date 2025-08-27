using StackExchange.Redis;

namespace WebAPI.Features.Cache;
using WebAPI.Annotation;

[Configuration]
public class RedisConfig
{
    private readonly IConnectionMultiplexer _muxer;
    private readonly IDatabase _db;
    private readonly ConfigurationOptions _conf;

    public RedisConfig(IConfiguration config)
    {
        String url = config["Redis:url"] ?? "";
        Int32 port = Int32.Parse(config["Redis:port"]!);
        String password = config["Redis:password"] ?? "";
        String user  = config["Redis:user"] ?? "";
        
        Console.WriteLine($"Redis URL: {url}, Port: {port}, Password: {password}, User: {user}");
        _conf = new ConfigurationOptions
        {
            EndPoints= { {url, port} },
            User = user,
            Password = password,
        };
        _muxer = ConnectionMultiplexer.Connect(_conf);
        _db = _muxer.GetDatabase();
    }

    public IDatabase redis() => _db;
}
