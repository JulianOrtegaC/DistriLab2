using StackExchange.Redis;
using System.Text.Json;
using IDatabase = StackExchange.Redis.IDatabase;

namespace DistriLab2.Service
{
    public class CacheService : ICacheService
    {
        private IDatabase _cacheDb;
        public CacheService()
        {
            var configurationOptions = ConfigurationOptions.Parse("cacheUniversity.redis.cache.windows.net:6380,password=M6xT5dikQvSdIy87qLWLDEUQ0QGuWR6GaAzCaONPn5s=,ssl=True,abortConnect=False");
            configurationOptions.SyncTimeout = 40000; // Aumenta el tiempo de espera a 10 segundos

            var redisClient = ConnectionMultiplexer.Connect(configurationOptions);

            _cacheDb = redisClient.GetDatabase();
        }
        public T GetData<T>(string key)
        {
            var value = _cacheDb.StringGet(key);
            if (!string.IsNullOrEmpty(value))
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDb.KeyExists(key);
            if (_exist)
            {
                return _cacheDb.KeyDelete(key);
            }
            return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationtime)
        {
            var expireTime = expirationtime.DateTime.Subtract(DateTime.Now);
            var isSet = _cacheDb.StringSet(key, JsonSerializer.Serialize(value), expireTime);
            return isSet;
        }
    }
}
