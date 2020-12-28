using System.Collections.Generic;
using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedLock
{
    public class LockObject : ILockObject

    {
        public bool IsAcquired => _lockFactory != null;

        private ILockFactory _lockFactory;
        private readonly KeyValuePair<string, string> _keyValue;

        public LockObject(ILockFactory lockFactory, string key, string value)
        {
            _lockFactory = lockFactory;
            _keyValue = new KeyValuePair<string, string>(key, value);
        }

        public bool Release()
        {
            if (!IsAcquired) return false;

            try
            {
                return _lockFactory.ReleaseLock(_keyValue.Key, _keyValue.Value);
            }
            finally
            {
                _lockFactory = null;
            }
        }

        public async Task<bool> ReleaseAsync()
        {
            if (!IsAcquired) return false;

            try
            {
                return await _lockFactory.ReleaseLockAsync(_keyValue.Key, _keyValue.Value);
            }
            catch
            {
                throw new DistributedLockException("An error happened on releasing the lock");
            }
            finally
            {
                _lockFactory = null;
            }
        }

        public void Dispose()
        {
            try
            {
                Release();
            }
            catch
            {
                throw new DistributedLockException("An error happened on releasing the lock");
            }
        }
    }
}