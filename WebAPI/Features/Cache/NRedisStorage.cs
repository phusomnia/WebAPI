using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Aggregation;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using WebAPI.Annotation;

namespace WebAPI.Features.Cache;

[Component]
public class NRedisStorage : ICacheBase
{
    private readonly IDatabase _redis;

    public NRedisStorage(RedisConfig config)
    {
        _redis = config.redis();
    }

    public Object test()
    {
        var user1 = new
        {
            name = "Paul John",
            email = "paul.john@example.com",
            age = 42,
            city = "London"
        };

        var user2 = new
        {
            name = "Eden Zamir",
            email = "eden.zamir@example.com",
            age = 29,
            city = "Tel Aviv"
        };

        var user3 = new
        {
            name = "Paul Zamir",
            email = "paul.zamir@example.com",
            age = 35,
            city = "Tel Aviv"
        };

        // Drop
        try
        {
            _redis.FT().DropIndex("idx:users");
        }
        catch (RedisServerException ex) when (ex.Message.Contains("Unknown Index name"))
        {
            throw new Exception(ex.Message);
        }

        var schema = new Schema()
                        .AddTextField(new FieldName("$.name", "name"))
                        .AddTagField(new FieldName("$.city", "city"))
                        .AddNumericField(new FieldName("$.age", "age"));

        Boolean indexCreated = _redis.FT().Create(
            "idx:users",
            new FTCreateParams()
            .On(IndexDataType.JSON)
            .Prefix("user:"),
            schema
        );

        Boolean user1Set = _redis.JSON().Set("user:1", "$", user1);
        Boolean user2Set = _redis.JSON().Set("user:2", "$", user2);
        Boolean user3Set = _redis.JSON().Set("user:3", "$", user3);

        // Query data
        SearchResult findPaulResult = _redis.FT().Search(
            "idx:users",
            new("Paul @age:[30 50]") 
        );
        string findResult = "[" + string.Join(",", findPaulResult.Documents.Select(x => x["json"])) + "]";
        var jsonString = JsonSerializer.Deserialize<object>(findResult);

        // Query aggregation
        AggregationRequest aggRequest = new AggregationRequest("*").GroupBy("@city", Reducers.Count().As("count"));
        AggregationResult aggResult = _redis.FT().Aggregate("idx:users", aggRequest);
        IReadOnlyList<Dictionary<string, RedisValue>> resultsList = aggResult.GetResults();

        var formatedResult = new List<Object>();
        for (var i = 0; i < resultsList.Count; i++)
        {
            Dictionary<string, RedisValue> item = resultsList.ElementAt(i);
            formatedResult.Add(new
            {
                city = item["city"].ToString(),
                count = item["count"].ToString()
            });
        }

        return formatedResult;
    }

    public Boolean set(string key, object value)
    {
        RedisValue redisValue = (RedisValue)JsonSerializer.Serialize(value);
        Boolean result = _redis.StringSet(key, redisValue);
        return result;
    }

    public Object? get(string key)
    {
        RedisValue value = _redis.StringGet(key);
        Console.WriteLine($"Value: {value.ToString()}");

        if (value.IsNullOrEmpty)
        {
            throw new KeyNotFoundException($"The key '{key}' was not found");
        }

        return JsonSerializer.Deserialize<Object>(value!);
    }
}
