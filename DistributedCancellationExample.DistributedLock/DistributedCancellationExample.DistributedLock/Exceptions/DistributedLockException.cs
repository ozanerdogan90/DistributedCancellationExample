using System;

namespace DistributedCancellationExample.DistributedLock
{
    internal class DistributedLockException : Exception
    {
        public DistributedLockException()
        {
        }

        public DistributedLockException(string message) : base(message)
        {
        }

        public DistributedLockException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}