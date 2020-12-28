using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedLock
{
    public interface ILockObject : IDisposable
    {
        bool IsAcquired { get; }

        bool Release();

        Task<bool> ReleaseAsync();
    }
}