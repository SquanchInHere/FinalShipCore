using FinalShipCore.Runtime.Contracts.Modules;
using FinalShipCore.Runtime.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FinalShipCore.Runtime.Modules
{
    public class ModuleComposer : GameModule, IModuleComposer
    {
        [SerializeField] private List<ModulePair> modulesList = new List<ModulePair>();

        private readonly ModuleRelationRegistry registry = new();
        private bool isConnected;
        private readonly List<(IModuleSignalSource signal, Action<string> handler)> subscriptions = new();

        public IModuleRelations Relations => registry;

        public override void Initialize()
        {
            if (modulesList == null
                || modulesList.Any(
                    item =>
                    item == null
                    || string.IsNullOrWhiteSpace(item.key)
                    || item.source == null
                    || item.target == null
                    )
                )
            {
                throw new InvalidOperationException("ModuleComposer: Invalid ModulePair configuration.");
            }


            Dictionary<(GameModule, string), ModuleRelationType> relationTypes = new();

            HashSet<(GameModule, string, GameModule)> uniqueRelations = new();

            foreach (ModulePair item in modulesList) 
            {
                var id = (item.source, item.key);
                var relation = (item.source, item.key, item.target);

                if (!string.IsNullOrWhiteSpace(item.command) ^ !string.IsNullOrWhiteSpace(item.signal))
                {
                    string emptyVariableName = string.IsNullOrWhiteSpace(item.command) ? nameof(item.command) : nameof(item.signal);
                    throw new InvalidOperationException($"ModuleComposer: Empty variable {emptyVariableName} in {item.key}");
                }

                if (!string.IsNullOrWhiteSpace(item.signal) && item.source is not IModuleSignalSource)
                {
                    throw new InvalidOperationException($"ModuleComposer: Source {item.key} is not IModuleSignalSource");
                }

                if (!string.IsNullOrWhiteSpace(item.command) && item.target is not IModuleCommandReceiver)
                {
                    throw new InvalidOperationException($"ModuleComposer: Target {item.key} is not IModuleCommandReceiver");
                }

                if (!Enum.IsDefined(typeof(ModuleRelationType), item.relationType))
                {
                    throw new InvalidOperationException($"ModuleComposer: Relation Type is not defined!");
                }

                if (!uniqueRelations.Add(relation))
                {
                    throw new InvalidOperationException($"ModuleComposer: Duplicate relation '{item.key}'.");
                }

                if (!relationTypes.TryGetValue(id, out ModuleRelationType existingType))
                {
                    relationTypes.Add(id, item.relationType);
                    continue;
                }

                if (existingType != item.relationType)
                {
                    throw new InvalidOperationException($"ModuleComposer: Conflicting relation type '{item.key}'.");
                }

                if (item.relationType != ModuleRelationType.HasMany)
                {
                    throw new InvalidOperationException($"ModuleComposer: Relation '{item.key}' must have one target.");
                }
            }
        }

        public override void Begin() 
        {
            foreach (ModulePair pair in modulesList)
            {
                if (pair.relationType == ModuleRelationType.HasMany)
                {
                    var targets = Relations.GetMany<IGameModule>(
                        pair.source, pair.key);

                    Debug.Log($"[Composer] {pair.key}: {targets.Count} modules");

                    continue;
                }

                var target = Relations.Get<IGameModule>(
                    pair.source, pair.key);

                Debug.Log($"[Composer] {pair.key}: Found = {target != null}");
            }
        }

        public override void Shutdown() 
        {
            Disconnect();
        }

        public void Connect()
        {
            if (isConnected) return;
            
            registry.Clear();

            try
            {
                foreach (ModulePair pair in modulesList)
                {
                    registry.Add(pair.source, pair.key, pair.target);

                    if (!string.IsNullOrWhiteSpace(pair.signal) && pair.source is IModuleSignalSource source)
                    {
                        Action<string> handler = message =>
                        {
                            if (!string.Equals(message, pair.signal, StringComparison.Ordinal)) return;

                            if (pair.target is IModuleCommandReceiver receiver)
                            {
                                bool success = receiver.ExecuteCommand(new ModuleCommand(pair.command, pair.argument));

                                if (!success) Debug.LogWarning($"Module Composer: Relation  '{pair.key}' - Command '{pair.command}' failed.");
                            }
                        };

                        source.SignalRaised += handler;

                        subscriptions.Add((source, handler));
                    }
                }
            } 
            catch
            {
                Disconnect();
                throw;
            }

            isConnected = true;
        }

        public void Disconnect()
        {
            foreach (var item in subscriptions)
            {
                item.signal.SignalRaised -= item.handler;
            }

            subscriptions.Clear();
            registry.Clear();
            isConnected = false;
        }
    }
}
