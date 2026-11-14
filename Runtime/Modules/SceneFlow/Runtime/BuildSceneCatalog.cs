using System.Linq;
using UnityEngine.SceneManagement;

namespace FinalShip.Modules.SceneFlow
{
    public class BuildSceneCatalog
    {
        public string[] GetScenes()
        {
            int sceneCount = SceneManager.sceneCountInBuildSettings;
            string[] scenePaths = new string[sceneCount];

            for (int i = 0; i < sceneCount; i++)
            {
                scenePaths[i] = SceneUtility.GetScenePathByBuildIndex(i);
            }

            return scenePaths;
        }

        public bool ContainScene(string scenePath)
        {
            if (string.IsNullOrWhiteSpace(scenePath))
                return false;

            return GetScenes().Contains(scenePath);
        }
    }
}
