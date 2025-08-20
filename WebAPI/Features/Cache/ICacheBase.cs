using StackExchange.Redis;

namespace WebAPI.Features.Cache;

public interface ICacheBase
{
    Object test();
    Boolean set(string key, object value);
    Object? get(string key);
}
