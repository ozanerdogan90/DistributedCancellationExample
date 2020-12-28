using System;

namespace DistributedCancellationExample.DistributedLock.Configurations
{
    public class DistributedLockConfiguration
    {
        public DistributedLockConfiguration(TimeSpan lockExpiry, int maxRetries)
        {
            LockExpiry = lockExpiry;
            MaxRetries = maxRetries;
        }

        public TimeSpan LockExpiry { get; } = TimeSpan.FromSeconds(30);
        public int MaxRetries { get; } = 10;
    }
}