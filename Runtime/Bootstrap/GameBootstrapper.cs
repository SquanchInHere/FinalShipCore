
using FinalShipCore.Runtime.Core;
using FinalShipCore.Runtime.Contracts.Modules;
using FinalShipCore.Runtime.Contracts.Loading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using FinalShipCore.Runtime.Modules;
using UnityEngine.Localization.Settings;

namespace FinalShipCore.Runtime.Bootstrap
{
    [DisallowMultipleComponent]
    public sealed class GameBootstrapper : MonoBehaviour, ISceneStartup
    {
        [SerializeField] private GameModule[] modules = Array.Empty<GameModule>();
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private LocalizationSettings defaultLocalizationSettings;

        private readonly List<IGameModule> initializedModules = new List<IGameModule>();
        private readonly List<IModuleComposer> connectedComposer = new List<IModuleComposer>();
        private readonly LoadingPipeline loadingPipeline = new LoadingPipeline();
        private readonly TaskCompletionSource<bool> taskComplition = 
            new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously
            );

        public Task StartupCompletion => taskComplition.Task;

        private async void Start()
        {
            CancellationToken cancellationToken = destroyCancellationToken;

            try
            {
                ValidateModules(); 
                ConfigureLocalization();
                ConfigureModuleInputs();
                InitializedModules();
                ConnectComposer();
                ConfigureModuleRelations();
                await loadingPipeline.ExecuteAsync(CollectLoadingTasks(), cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
                BeginModules();
                taskComplition.TrySetResult(true);

                Debug.Log("Modules: started successes");
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                taskComplition.TrySetCanceled();
                Debug.LogWarning("Loading pipline is canceled request!");
            }
            catch (Exception exception)
            {
                taskComplition.TrySetException(exception);
                Debug.LogException(exception, this);
                DisconnectComposer();
                ShutdownModules();
            }
        }

        private void OnDestroy()
        {
            taskComplition.TrySetCanceled();
            DisconnectComposer();
            ShutdownModules();
        }

        private void ValidateModules()
        {
            HashSet<IGameModule> uniqueModules = new HashSet<IGameModule>();

            for (int i = 0; i < modules.Length; i++)
            {
                GameModule module = modules[i];

                if (module == null) throw new InvalidOperationException($"Modules {i} not implemented");

                if (!module.isActiveAndEnabled) 
                    throw new InvalidOperationException($"Modules {module.name} on object must be enabled!");

                if (!uniqueModules.Add(module)) 
                    throw new InvalidOperationException($"Modules {module.name} allready exists  on object!");

            }
        }

        private void ConnectComposer()
        {
            foreach (var module in initializedModules)
            {
                if (module is IModuleComposer composer)
                {
                    connectedComposer.Add(composer);
                    composer.Connect();
                }
            }
        }

        private void DisconnectComposer()
        {
            for (int i = connectedComposer.Count - 1; i >= 0; i--)
            {
                try
                {
                    connectedComposer[i].Disconnect();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }

            connectedComposer.Clear();
        }

        private void InitializedModules()
        {
            foreach (GameModule module in modules)
            {
                module.Initialize();
                initializedModules.Add(module);
            }
        }

        private void BeginModules()
        {
            foreach (IGameModule module in initializedModules)
            {
                module.Begin();
            }
        }

        private void ShutdownModules()
        {
            for (int i = initializedModules.Count - 1; i >= 0; i--)
            {
                try
                {
                    initializedModules[i].Shutdown();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }

            initializedModules.Clear();
        }

        private void ConfigureModuleInputs()
        {
            foreach (IGameModule module in modules)
            {
                if (module is IRequireInput inputConsumer)
                {
                    if (inputActions == null) 
                        throw new InvalidOperationException("GameBootstrapper: Input Actions asset is not assigned.");
                    
                    inputConsumer.ConfigureInput(inputActions);
                }
            }
        }

        private void ConfigureModuleRelations()
        {
            ModuleComposer composer = null;

            foreach (GameModule module in modules)
            {
                if (module is ModuleComposer found)
                {
                    if (composer != null)
                        throw new InvalidOperationException("GameBootstrapper: Multiple ModuleComposer instances.");

                    composer = found;
                }
            }

            if (composer == null)
                return;

            foreach (GameModule module in modules)
            {
                if (module is IRequireModuleRelations consumer)
                {
                    consumer.ConfigureRelations(composer.Relations);
                }
            }
        }

        private IReadOnlyList<ILoadingTask> CollectLoadingTasks()
        {
            List<ILoadingTask> loadingTasks = new List<ILoadingTask>();

            foreach (IGameModule module in initializedModules)
            {
               if (module is ILoadingTask loadingTask)
                {
                    loadingTasks.Add(loadingTask);
                }
            }

            return loadingTasks;
        }

        private void ConfigureLocalization()
        {
            if (LocalizationSettings.HasSettings)
                return;

            if (defaultLocalizationSettings == null)
            {
                throw new InvalidOperationException(
                    "GameBootstrapper: Default Localization Settings are not assigned."
                );
            }

            LocalizationSettings.Instance = defaultLocalizationSettings;
        }
    }
}
