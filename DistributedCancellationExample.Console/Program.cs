using DistributedCancellationExample.Console.DependencyInjection;
using DistributedCancellationExample.Console.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCancellationExample.Console
{
    internal class Program
    {
        private static IConfiguration Configuration;
        private static ServiceProvider ServiceProvider;
        private static Guid DeviceId = Guid.NewGuid();
        private static int CommandCount = 300;

        private static void Main(string[] args)
        {
            ConfigureHostConfiguration();
            ConfigureServices();

            IDeviceService deviceService = ServiceProvider.GetService<IDeviceService>();
            IDatabaseAsync databaseAsync = ServiceProvider.GetService<IDatabaseAsync>();
            ILogger logger = ServiceProvider.GetService<ILogger>();

            SeedAction(deviceService).GetAwaiter().GetResult();

            long totalExecutedCommand = databaseAsync.StringIncrementAsync($"{DeviceId}-Restart").GetAwaiter().GetResult();

            logger.LogInformation($"Total fired event count : {CommandCount} {Environment.NewLine}Total executed command count : {totalExecutedCommand - 1}");
        }

        private static void ConfigureHostConfiguration()
        {
            Configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json", false)
                 .AddUserSecrets<Program>()
                 .Build();
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();
            services.RegisterServices(Configuration);
            ServiceProvider = services.BuildServiceProvider();
        }

        private static Task SeedAction(IDeviceService deviceService)
        {
            IEnumerable<Device> devices = Enumerable.Repeat(new Device() { Id = DeviceId }, CommandCount);

            return Task.WhenAll(devices.Select(device => deviceService.ExecuteAsync(device, Domain.Action.Restart, CancellationToken.None)));
        }
    }
}