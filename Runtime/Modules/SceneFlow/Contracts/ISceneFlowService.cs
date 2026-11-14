using System;

namespace FinalShip.Modules.SceneFlow
{
    public interface ISceneFlowService
    {
        SceneFlowState State { get; }
        bool IsLoading { get; }

        event Action<SceneFlowState> StateChanged;

        public bool TryLoadScene(string scenePath);
    }
}
