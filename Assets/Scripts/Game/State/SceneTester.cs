using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public abstract class SceneTester : MainGameState
    {
        [SerializeField]
        private string[] _testScenes;

        public string[] TestScenes => _testScenes;

        public void SetScene(string sceneName)
        {
            CurrentSceneName = sceneName;
        }
    }
}
