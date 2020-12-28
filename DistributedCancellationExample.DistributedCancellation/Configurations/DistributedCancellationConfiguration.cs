using System;

namespace DistributedCancellationExample.DistributedCancellation.Configurations
{
    public class DistributedCancellationConfiguration
    {
        public DistributedCancellationConfiguration(int maxRetries, TimeSpan maximumRetryDelay)
        {
            MaxRetries = maxRetries;
            MaximumRetryDelay = maximumRetryDelay;
        }

        public int MaxRetries { get; } = 10;
        public TimeSpan MaximumRetryDelay { get; } = TimeSpan.FromMilliseconds(1200);
    }
}