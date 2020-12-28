using DistributedCancellationExample.DistributedCancellation.Configurations;
using DistributedCancellationExample.DistributedCancellation.Exceptions;
using DistributedCancellationExample.DistributedLock;
using Microsoft.Extensions.Logging;
using Polly;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedCancellation
{
    public class DistributedCancellationProcessor : IDistributedCancellationProcessor
    {
        private readonly ILockFactory _lockFactory;
        private readonly ILogger _logger;
        private readonly ISubscriber _redisSubscriber;
        private readonly DistributedCancellationConfiguration _distributedCancellationConfiguration;
        private readonly IDatabaseAsync _databaseAsync;

        public DistributedCancellationProcessor(ILockFactory lockFactory, ILogger logger, ISubscriber redisSubscriber, DistributedCancellationConfiguration distributedCancellationConfiguration, IDatabaseAsync databaseAsync)
        {
            _lockFactory = lockFactory;
            _logger = logger;
            _redisSubscriber = redisSubscriber;
            _distributedCancellationConfiguration = distributedCancellationConfiguration;
            _databaseAsync = databaseAsync;
        }

        public Task ExecuteAsync(string key, Func<CancellationToken, Task> action)
        {
            return ExecuteAsync<object>(
                 key,
                 async (CancellationToken ct) =>
                 {
                     await action(ct).ConfigureAwait(false);

                     return null;
                 }
             );
        }

        public async Task<TResponse> ExecuteAsync<TResponse>(string key, Func<CancellationToken, Task<TResponse>> action)
        {
            ILockObject @lock = await _lockFactory.AcquireLockAsync(key);
            DateTime processStarted = DateTime.UtcNow;

            try
            {
                using var subscribedCancellationSource = new CancellationTokenSource();

                await _redisSubscriber.SubscribeAsync(
                        key,
                        (channel, _) =>
                        {
                            _logger.LogInformation(
                                $"A cancellation message was received for key: {key}"
                            );

                            if (!subscribedCancellationSource.IsCancellationRequested)
                            {
                                subscribedCancellationSource.Cancel();
                            }
                        }
                    ).ConfigureAwait(false);

                TResponse response = await Policy
                     .Handle<Exception>()
                     .WaitAndRetryAsync(_distributedCancellationConfiguration.MaxRetries, _ => _distributedCancellationConfiguration.MaximumRetryDelay)
                     .ExecuteAsync(() =>
                     {
                         return action(subscribedCancellationSource.Token);
                     });

                _logger.LogDebug($"Finished processing for key: {key}");

                await _databaseAsync.StringIncrementAsync(key);

                return response;
            }
            catch (Exception ex)
            {
                throw new DistributedCancellationException("An exception happened", ex);
            }
            finally
            {
                await @lock.ReleaseAsync();

                await _redisSubscriber.UnsubscribeAsync(key).ConfigureAwait(false);
            }
        }
    }
}