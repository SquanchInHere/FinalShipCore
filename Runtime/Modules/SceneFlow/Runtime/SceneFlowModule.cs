using FinalShipCore.Runtime.Contracts.Modules;
using FinalShipCore.Runtime.Core;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace FinalShip.Modules.SceneFlow
{
    public sealed class SceneFlowModule : GameModule, IModuleCommandReceiver
    {
        [SerializeField] private LoadingScreenSettings loadingScreenSettings;
        [SerializeField] private PanelRenderer panelRenderer;

        private BuildSceneCatalog catalog;
        private SceneTransitionService transitionService;
        private LoadingScreens loadingScreens = new LoadingScreens();
        public ISceneFlowService Service => transitionService;

        private bool isLoadingUiReady;
        private bool isSceneReady;

        public void OnLoadingUiReload(PanelRenderer renderer, VisualElement root, int version)
        {
            loadingScreens.Bind(root, loadingScreenSettings);
            isLoadingUiReady = true;
            LoadingScreenVisibility();
        }

        public override void Begin()
        {
            Debug.Log("[SceneFlowModule]Module is started");
        }

        public override void Initialize()
        {
            isLoadingUiReady = false;
            isSceneReady = false;

            catalog = new BuildSceneCatalog();
            transitionService = new SceneTransitionService(catalog);

            if (panelRenderer == null) 
                throw new InvalidOperationException("[SceneFlowModule] PanelRender is not assigned!");

            if (loadingScreenSettings == null)
                throw new InvalidOperationException("[SceneFlowModule] Screen Settings is not implemented !");

            if (loadingScreenSettings.ScreenAsset == null)
                throw new InvalidOperationException("[SceneFlowModule] ScreenAsset is not assigned in to Screen Settings!");

            panelRenderer.visualTreeAsset = loadingScreenSettings.ScreenAsset;

            panelRenderer.RegisterUIReloadCallback(OnLoadingUiReload);

            _ = ObserveStartupAsync(destroyCancellationToken);
        }

        public override void Shutdown()
        {
            if (panelRenderer != null) panelRenderer.UnregisterUIReloadCallback(OnLoadingUiReload);
            Debug.Log("[SceneFlowModule]Module Shutdown");
        }

        public bool ExecuteCommand(ModuleCommand command)
        {
            Debug.Log($"[SceneFlow] Command={command.Name}, Argument={command.Argument}");
            if (!Enum.TryParse<SceneCommandType>(command.Name, false, out var action) || !Enum.IsDefined(typeof(SceneCommandType), action)) return false;
            
            return transitionService.TryLoadScene(command.Argument);
        }

        private void LoadingScreenVisibility()
        {
            if (!isLoadingUiReady) return;

            if (isSceneReady) loadingScreens.Hide();
            else loadingScreens.Show();
        }

        private async Task ObserveStartupAsync(CancellationToken cancellationToken)
        {
            try
            {
                await transitionService.WaitForStartupAsync(gameObject.scene);

                cancellationToken.ThrowIfCancellationRequested();

                isSceneReady = true;

                LoadingScreenVisibility();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            { }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
        }
    }
}
