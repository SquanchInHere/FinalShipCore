using System.Threading;
using System.Threading.Tasks;

namespace FinalShipCore.Runtime.Contracts.Loading
{
    public interface ILoadingTask
    {
        float? Progress { get; }

        Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
