using System;

namespace DistributedCancellationExample.DistributedCancellation.Exceptions
{
    public class DistributedCancellationException : Exception
    {
        public DistributedCancellationException()
        {
        }

        public DistributedCancellationException(string message) : base(message)
        {
        }

        public DistributedCancellationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}