using System.Threading.Tasks;

namespace FinalShipCore.Runtime.Contracts.Loading
{
    public interface ISceneStartup
    {
        Task StartupCompletion { get; }
    }
}
