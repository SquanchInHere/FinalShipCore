
namespace FinalShipCore.Runtime.Contracts.Modules
{
    public interface IModuleCommandReceiver
    {
        bool ExecuteCommand(ModuleCommand command);
    }
}
