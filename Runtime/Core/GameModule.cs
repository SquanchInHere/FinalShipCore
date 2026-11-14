using FinalShipCore.Runtime.Contracts.Modules;
using UnityEngine;

namespace FinalShipCore.Runtime.Core
{
    public abstract class GameModule : MonoBehaviour, IGameModule
    {
        public abstract void Initialize();

        public abstract void Begin();

        public abstract void Shutdown();
    }
}
