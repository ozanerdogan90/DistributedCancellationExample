using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;

namespace DistributedCancellationExample.Console.DependencyInjection
{
    public static class RedisRegistrations
    {
        public static void RegisterRedis(this IServiceCollection services, IConfiguration configuration)
        {
            ConnectionMultiplexer redis = GetRedisConnection(configuration).Value;

            services.AddSingleton<IConnectionMultiplexer>(redis);
            services.AddSingleton<IDatabaseAsync>(redis.GetDatabase());
            services.AddSingleton<ISubscriber>(redis.GetSubscriber());
        }

        private static Lazy<ConnectionMultiplexer> GetRedisConnection(IConfiguration config)
        {
            return new Lazy<ConnectionMultiplexer>(() =>
             {
                 string cacheConnection = config.GetConnectionString("Redis");
                 return ConnectionMultiplexer.Connect(cacheConnection);
             });
        }
    }
}