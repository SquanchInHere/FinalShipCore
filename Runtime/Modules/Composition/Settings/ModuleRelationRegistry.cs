using FinalShipCore.Runtime.Contracts.Modules;
using System;
using System.Collections.Generic;

namespace FinalShipCore.Runtime.Modules
{
    public sealed class ModuleRelationRegistry : IModuleRelations
    {
        private readonly Dictionary<(IGameModule source, string key), List<IGameModule>> relations = new();

        public void Add(IGameModule source, string key, IGameModule target)
        {
            var id = (source, key);

            if (!relations.TryGetValue(id, out var targets))
            {
                targets = new List<IGameModule>();
                relations.Add(id, targets);
            }

            targets.Add(target);
        }

        public T Get<T>(IGameModule source, string key) where T : IGameModule
        {
            var results = GetMany<T>(source, key);


            return results.Count == 1 ? results[0] : default;
        }

        public IReadOnlyList<T> GetMany<T>(IGameModule source, string key) where T : IGameModule
        {
            var id = (source, key);

            if (!relations.TryGetValue(id, out var targets))
                return Array.Empty<T>();

            List<T> results = new();

            foreach (IGameModule target in targets)
            {
                if (target is T typed)
                    results.Add(typed);
            }

            return results;
        }

        public void Clear()
        {
            relations.Clear();
        }

        public T GetPath<T>(IGameModule source, string path) where T : IGameModule
        {
            if (source == null || string.IsNullOrWhiteSpace(path))
                return default;

            string[] keys = path.Split('.');
            IGameModule current = source;

            foreach (string key in keys)
            {
                if (string.IsNullOrWhiteSpace(key))
                    return default;

                current = Get<IGameModule>(current, key);

                if (current == null)
                    return default;
            }

            return current is T result ? result : default;
        }
    }
}
