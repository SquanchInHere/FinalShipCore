using FinalShipCore.Runtime.Contracts.Loading;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace FinalShip.Modules.SceneFlow
{
    public class SceneTransitionService : ISceneFlowService
    {
        private BuildSceneCatalog catalog;
        private AsyncOperation currentOperation;
        private SceneFlowState currentSceneFlowState = SceneFlowState.Idle;

        public SceneTransitionService(BuildSceneCatalog catalog)
        {
            this.catalog = catalog;
        }

        public SceneFlowState State => currentSceneFlowState;

        public event Action<SceneFlowState> StateChanged;

        public bool IsLoading => currentOperation != null && !currentOperation.isDone;

        public bool TryLoadScene(string scenePath)
        {
            if (IsLoading || !catalog.ContainScene(scenePath)) return false;

            try
            {
                currentOperation = SceneManager.LoadSceneAsync(scenePath);
            } catch(Exception ex)
            {
                Debug.LogException(ex);

                ChangeState(SceneFlowState.Failed);

                return false;
            }

            if (currentOperation == null)
            {
                Debug.LogError("[SceneFlow] Failed to start scene loading."); 
                ChangeState(SceneFlowState.Failed);
                return false;
            }

            currentOperation.completed += OnLoadCompleted;

            ChangeState(SceneFlowState.Loading);

            return true;
        }

        private void ChangeState(SceneFlowState newState)
        {
            if (currentSceneFlowState == newState) return;

            Debug.Log($"[SceneFlow] State: {currentSceneFlowState} → {newState}");

            currentSceneFlowState = newState;

            StateChanged?.Invoke(currentSceneFlowState);

        }

        private void OnLoadCompleted(AsyncOperation operation)
        {
            ChangeState(SceneFlowState.Idle);
            Debug.Log("[SceneFlow] Scene loading operation completed.");
        }

        private static ISceneStartup FindSceneStartup(Scene scene)
        {
            ISceneStartup contract = null;

            if (!scene.IsValid() || !scene.isLoaded)
                throw new InvalidOperationException($"[SceneFlow] {scene.name} is not {(!scene.IsValid() ? "valid" : "loaded")} ");

            foreach (GameObject root in scene.GetRootGameObjects())
            {
                MonoBehaviour[] components = root.GetComponentsInChildren<MonoBehaviour>(true);

                foreach (MonoBehaviour component in components)
                {
                    if (component is ISceneStartup sceneStartup)
                    {
                        if (contract != null)
                            throw new InvalidOperationException($"[SceneFlow] scene: {scene.name} Contract is already exists!");

                        if (!component.isActiveAndEnabled)
                            throw new InvalidOperationException($"[SceneFlow] scene: {scene.name} Component is not enabled!");

                        contract = sceneStartup;
                    }
                }
            }

            if (contract == null)
                throw new InvalidOperationException($"[SceneFlow] ISceneStartup in scene: {scene.name} is not exists!");

            return contract;
        }

        public async Task WaitForStartupAsync(Scene scene)
        {
            ISceneStartup contract = FindSceneStartup(scene);
            await contract.StartupCompletion;
        }
    }
}
