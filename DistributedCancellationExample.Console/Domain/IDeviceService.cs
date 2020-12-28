using System.Threading;
using System.Threading.Tasks;

namespace DistributedCancellationExample.Console.Domain
{
    public interface IDeviceService
    {
        Task ExecuteAsync(Device device, Action action, CancellationToken ct);
    }
}