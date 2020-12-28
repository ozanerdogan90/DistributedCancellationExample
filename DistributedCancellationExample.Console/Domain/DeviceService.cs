using DistributedCancellationExample.DistributedCancellation;
using DistributedCancellationExample.DistributedCancellation.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCancellationExample.Console.Domain
{
    public class DeviceService : IDeviceService
    {
        private readonly Random _randomGenerator = new Random(Guid.NewGuid().GetHashCode());
        private readonly IDistributedCancellationProcessor _distributedCancellationProcessor;
        private readonly ILogger _logger;

        public DeviceService(IDistributedCancellationProcessor distributedCancellationProcessor, ILogger logger)
        {
            _distributedCancellationProcessor = distributedCancellationProcessor;
            _logger = logger;
        }

        public async Task ExecuteAsync(Device device, Action action, CancellationToken cancellationToken)
        {
            try
            {
                string key = $"{device.Id}-{action}";
                cancellationToken.ThrowIfCancellationRequested();
                await _distributedCancellationProcessor.ExecuteAsync(key, (distributedCancellation) =>
                {
                    CancellationToken token = CancellationTokenSource.CreateLinkedTokenSource(
                        distributedCancellation,
                        cancellationToken
                    ).Token;

                    return ExecuteCommand(device, action, token);
                });
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation($"Operation cancelled exception happened");
            }
            catch (DistributedCancellationException dce)
            {
                _logger.LogWarning(dce, $"Distributed cancellation happened");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An exception happened");
            }
        }

        private async Task ExecuteCommand(Device device, Action action, CancellationToken token)
        {
            //// Dummy wait

            int delayMilliSeconds = _randomGenerator.Next(
                    (int)TimeSpan.FromSeconds(0.5).TotalMilliseconds,
                    (int)TimeSpan.FromSeconds(5).TotalMilliseconds
                );

            await Task.Delay(delayMilliSeconds, token).ConfigureAwait(false);
            _logger.LogInformation($"Command is executed with id : {device.Id}");
        }
    }
}