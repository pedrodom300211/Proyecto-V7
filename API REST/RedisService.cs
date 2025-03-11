using Newtonsoft.Json;
using StackExchange.Redis;

public class RedisService
{
    private readonly ConnectionMultiplexer _connection;
    private readonly IDatabase _database;

    public RedisService()
    {
        _connection = ConnectionMultiplexer.Connect(
            new ConfigurationOptions
            {
                EndPoints = { { "redis-19684.c61.us-east-1-3.ec2.redns.redis-cloud.com", 19684 } },
                User = "default",
                Password = "jm8XGp6DR7vZ1rRYifQDxbERijVk0NF8"
            }
        );

        _database = _connection.GetDatabase();
    }

    public void SaveJson<T>(string key, T data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        _database.StringSet(key, jsonData);
    }

    public T GetJson<T>(string key)
    {
        var jsonData = _database.StringGet(key);
        return jsonData.IsNullOrEmpty ? default : JsonConvert.DeserializeObject<T>(jsonData);
    }
}
