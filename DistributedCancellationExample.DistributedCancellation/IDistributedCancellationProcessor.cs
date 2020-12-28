using System;
using System.Threading;
using System.Threading.Tasks;

namespace DistributedCancellationExample.DistributedCancellation
{
    public interface IDistributedCancellationProcessor
    {
        Task ExecuteAsync(
            string key,
            Func<CancellationToken, Task> action
        );

        Task<TResponse> ExecuteAsync<TResponse>(
            string key,
            Func<CancellationToken, Task<TResponse>> action
        );
    }
}