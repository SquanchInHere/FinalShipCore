using UnityEngine.InputSystem;

namespace FinalShipCore.Runtime.Contracts.Modules
{
    public interface IRequireInput
    {
        void ConfigureInput(InputActionAsset inputActions);
    }
}
