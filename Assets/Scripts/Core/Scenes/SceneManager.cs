using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace pdxpartyparrot.Core.Scenes
{
    public sealed class SceneManager : SingletonBehavior<SceneManager>
    {
        [SerializeField]
        private string _mainSceneName = "main";

        private readonly List<string> _loadedScenes = new List<string>();

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }
#endregion

#region Load Scenes
        public IEnumerator<float> LoadSceneRoutine(string sceneName, bool setActive=false)
        {
            Debug.Log($"Loading scene '{sceneName}'...");

            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if(null == asyncOp) {
                yield break;
            }

            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }

            if(setActive) {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName));
            }

            _loadedScenes.Add(sceneName);

            Debug.Log($"Scene '{sceneName}' loaded...");
        }
#endregion

#region Unload Scenes
        public IEnumerator<float> UnloadSceneRoutine(string sceneName)
        {
            Debug.Log($"Unloading scene '{sceneName}'...");

            AsyncOperation asyncOp = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
            while(!asyncOp.isDone) {
                yield return asyncOp.progress;
            }

            _loadedScenes.Remove(sceneName);

            Debug.Log($"Scene '{sceneName}' unloaded...");
        }

        public IEnumerator<float> UnloadAllScenesRoutine()
        {
            Debug.Log("Unloading all scenes...");

            if(_loadedScenes.Count > 0) {
                float step = 1.0f / _loadedScenes.Count;

                int completed = 0;
                foreach(string sceneName in _loadedScenes) {
                    IEnumerator<float> runner = UnloadSceneRoutine(sceneName);
                    while(runner.MoveNext()) {
                        yield return (completed * step) + runner.Current * step;
                    }
                    completed++;
                }
            }

            UnityEngine.SceneManagement.SceneManager.SetActiveScene(UnityEngine.SceneManagement.SceneManager.GetSceneByName(_mainSceneName));
        }
#endregion

#region Reload Scene
        public IEnumerator ReloadMainSceneRoutine()
        {
            Debug.Log("Reloading...");

            IEnumerator runner = UnloadAllScenesRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            Debug.Log($"Loading main scene '{_mainSceneName}'");
            UnityEngine.SceneManagement.SceneManager.LoadScene(_mainSceneName);
        }

        public IEnumerator ReloadSceneRoutine(string sceneName, bool setActive)
        {
            Debug.Log($"Reloading scene '{sceneName}'");

            IEnumerator<float> runner = UnloadSceneRoutine(sceneName);
            while(runner.MoveNext()) {
                yield return null;
            }

            runner = LoadSceneRoutine(sceneName, setActive);
            while(runner.MoveNext()) {
                yield return null;
            }
        }
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.SceneManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Loaded Scenes", GUI.skin.box);
                    foreach(string loadedScene in _loadedScenes) {
                        GUILayout.Label(loadedScene);
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
