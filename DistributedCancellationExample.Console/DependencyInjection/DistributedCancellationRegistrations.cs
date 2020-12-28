using DistributedCancellationExample.DistributedCancellation;
using DistributedCancellationExample.DistributedCancellation.Configurations;
using DistributedCancellationExample.DistributedLock;
using DistributedCancellationExample.DistributedLock.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DistributedCancellationExample.Console.DependencyInjection
{
    public static class DistributedCancellationRegistrations
    {
        public static void RegisterDistributedCancellation(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterDistributedCancellationServices(services, configuration);
            RegisterDistributedLockingServices(services, configuration);
        }

        private static void RegisterDistributedCancellationServices(IServiceCollection services, IConfiguration configuration)
        {
            var dcConfiguration = new DistributedCancellationConfiguration(
                configuration.GetValue<int>("DistributedCancellation:MaxRetries"),
                configuration.GetValue<TimeSpan>("DistributedCancellation:MaxRetryDelay")
                );

            services.AddScoped<IDistributedCancellationProcessor, DistributedCancellationProcessor>();
            services.AddScoped(_ => dcConfiguration);
        }

        private static void RegisterDistributedLockingServices(IServiceCollection services, IConfiguration configuration)
        {
            var dcConfiguration = new DistributedLockConfiguration(
                configuration.GetValue<TimeSpan>("DistributedLock:LockExpiry"),
                configuration.GetValue<int>("DistributedLock:MaxRetries")
                );

            services.AddScoped<ILockFactory, RedisLockFactory>();
            services.AddScoped(_ => dcConfiguration);
        }
    }
}