using System.Collections.Generic;

namespace FinalShipCore.Runtime.Contracts.Modules
{
    public interface IModuleRelations
    {
        T Get<T>(IGameModule source, string key) where T : IGameModule;

        IReadOnlyList<T> GetMany<T>(IGameModule source, string key) where T : IGameModule;

        T GetPath<T>(IGameModule source, string path)
            where T : IGameModule;
    }
}
