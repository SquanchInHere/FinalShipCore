using System;

namespace FinalShipCore.Runtime.Contracts.Modules
{
    public interface IModuleSignalSource
    {
        event Action<string> SignalRaised;
    }
}
