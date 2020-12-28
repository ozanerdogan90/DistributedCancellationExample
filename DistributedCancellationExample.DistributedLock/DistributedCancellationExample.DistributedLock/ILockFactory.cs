using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedLock
{
    public interface ILockFactory
    {
        ILockObject AcquireLock(string key);

        Task<ILockObject> AcquireLockAsync(string key);

        Task<bool> ReleaseLockAsync(string key, string value);

        bool ReleaseLock(string key, string value);
    }
}