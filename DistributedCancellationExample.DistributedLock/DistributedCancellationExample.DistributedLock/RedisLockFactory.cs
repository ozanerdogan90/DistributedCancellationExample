using DistributedCancellationExample.DistributedLock.Configurations;
using Polly;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedLock
{
    public class RedisLockFactory : ILockFactory
    {
        private readonly IDatabaseAsync _database;
        private readonly DistributedLockConfiguration _distributedLockConfiguration;

        public RedisLockFactory(IDatabaseAsync database, DistributedLockConfiguration distributedLockConfiguration)
        {
            _database = database;
            _distributedLockConfiguration = distributedLockConfiguration ?? throw new ArgumentNullException(nameof(distributedLockConfiguration));
        }

        public ILockObject AcquireLock(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Key can not be null or empty.");

            string value = Guid.NewGuid().ToString();

            bool isSuccess = false;

            try
            {
                isSuccess = Policy
                    .Handle<Exception>()
                    .Retry(_distributedLockConfiguration.MaxRetries)
                    .Execute(() =>
                    {
                        return _database.LockTakeAsync(key, value, _distributedLockConfiguration.LockExpiry).GetAwaiter().GetResult();
                    });
            }
            catch (Exception ex)
            {
                throw new DistributedLockException($"Failed to set the key('{key}') to acquiring a lock.", ex);
            }

            return isSuccess ? new LockObject(this, key, value) : throw new DistributedLockException($"Failed to set the key('{key}') to acquiring a lock.");
        }

        public async Task<ILockObject> AcquireLockAsync(string key)
        {
            string value = Guid.NewGuid().ToString();

            bool isSuccess;

            try
            {
                isSuccess = await Policy
                   .Handle<Exception>()
                   .RetryAsync(_distributedLockConfiguration.MaxRetries)
                   .ExecuteAsync<bool>(() =>
                   {
                       return _database.LockTakeAsync(key, value, _distributedLockConfiguration.LockExpiry);
                   });
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DistributedLockException($"Failed to set the key('{key}') to acquiring a lock.", ex);
            }

            return isSuccess ? new LockObject(this, key, value) : throw new DistributedLockException($"Failed to set the key('{key}') to acquiring a lock.");
        }

        public bool ReleaseLock(string key, string value)
        {
            try
            {
                return _database.LockReleaseAsync(key, value).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new DistributedLockException($"Failed to delete the key('{key}') to release the lock.", ex);
            }
        }

        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
            try
            {
                return await _database.LockReleaseAsync(key, value);
            }
            catch (Exception ex)
            {
                throw new DistributedLockException($"Failed to delete the key('{key}') to release the lock.", ex);
            }
        }
    }
}