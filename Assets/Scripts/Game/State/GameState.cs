using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Core.Scenes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public abstract class GameState : MonoBehaviour
    {
        public string Name => name;

        [SerializeField]
        private string _initialSceneName;

        [SerializeField]
        [ReadOnly]
        private string _currentSceneName;

        public string CurrentSceneName
        {
            get => _currentSceneName;
            protected set => _currentSceneName = value;
        }

        public bool HasScene => !string.IsNullOrWhiteSpace(CurrentSceneName);

        [SerializeField]
        private bool _makeInitialSceneActive;

        public bool MakeInitialSceneActive => _makeInitialSceneActive;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _enterEffect;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _exitEffect;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _currentSceneName = _initialSceneName;
        }
#endregion

        public IEnumerator<float> LoadSceneRoutine()
        {
            if(!HasScene) {
                yield break;
            }

            IEnumerator<float> runner = SceneManager.Instance.LoadSceneRoutine(CurrentSceneName, MakeInitialSceneActive);
            while(runner.MoveNext()) {
                yield return runner.Current;
            }
        }

        public IEnumerator<float> UnloadSceneRoutine()
        {
            if(!HasScene) {
                yield break;
            }

            if(SceneManager.HasInstance) {
                IEnumerator<float> runner = SceneManager.Instance.UnloadSceneRoutine(CurrentSceneName);
                while(runner.MoveNext()) {
                    yield return runner.Current;
                }
            }
        }

        public void ChangeSceneAsync(string sceneName, Action onComplete)
        {
            StartCoroutine(ChangeSceneRoutine(sceneName, onComplete));
        }

        private IEnumerator ChangeSceneRoutine(string sceneName, Action onComplete)
        {
            IEnumerator<float> runner = UnloadSceneRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            CurrentSceneName = sceneName;

            runner = LoadSceneRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }

            onComplete?.Invoke();
        }

        public virtual IEnumerator<LoadStatus> OnEnterRoutine()
        {
            Debug.Log($"Enter State: {Name}");

            yield break;
        }

        public virtual void OnEnter()
        {
            if(null != _enterEffect) {
                _enterEffect.Trigger(DoEnter);
            } else {
                DoEnter();
            }
        }

        // called after enter effects
        protected virtual void DoEnter()
        {
        }

        public virtual IEnumerator<LoadStatus> OnExitRoutine()
        {
            Debug.Log($"Exit State: {Name}");

            yield break;
        }

        public virtual void OnExit()
        {
            // make sure the enter effect is stopped
            if(null != _enterEffect) {
                _enterEffect.KillTrigger();
            }

            if(null != _exitEffect) {
                _exitEffect.Trigger(DoExit);
            } else {
                DoExit();
            }
        }

        // called after exit effects
        protected virtual void DoExit()
        {
        }

        public virtual void OnResume()
        {
            Debug.Log($"Resume State: {Name}");
        }

        public virtual void OnPause()
        {
            Debug.Log($"Pause State: {Name}");
        }

        public virtual void OnUpdate(float dt)
        {
        }
    }
}
