using DistributedCancellationExample.Console.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DistributedCancellationExample.Console.DependencyInjection
{
    public static class DomainRegistrations
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterDistributedCancellation(configuration);
            services.RegisterRedis(configuration);
            RegisterDomainServices(services);
        }

        public static void RegisterDomainServices(IServiceCollection services)
        {
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            services.AddScoped<IDeviceService, DeviceService>();
            services.AddScoped<ILogger>(_ => loggerFactory.CreateLogger(nameof(DeviceService)));
        }
    }
}