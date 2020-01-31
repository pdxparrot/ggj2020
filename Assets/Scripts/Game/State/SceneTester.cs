using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public class SceneTester : MainGameState
    {
        [SerializeField]
        private string[] _testScenes;

        public string[] TestScenes => _testScenes;

        protected override bool InitializeClient()
        {
            // need to init the viewer before we start spawning players
            // so that they have a viewer to attach to
            InitViewer();

            if(!base.InitializeClient()) {
                Debug.LogWarning("Failed to initialize client!");
                return false;
            }

            return true;
        }

        public void InitViewer()
        {
            /*ViewerManager.Instance.AllocateViewers(1, GameStateManager.Instance.GameManager.GameData.ViewerPrefab);
            GameStateManager.Instance.GameManager.InitViewer();*/
        }

        public void SetScene(string sceneName)
        {
            CurrentSceneName = sceneName;
        }
    }
}
