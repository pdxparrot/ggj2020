using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Scenes;
using pdxpartyparrot.Core.Scripting;
using pdxpartyparrot.Core.Terrain;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;

using UnityEngine;

namespace pdxpartyparrot.Core.Loading
{
    public interface ILoadingManager
    {
        void ShowLoadingScreen(bool show);

        void UpdateLoadingScreen(float percent, string text);
    }

    public abstract class LoadingManager<T> : SingletonBehavior<T>, ILoadingManager where T: LoadingManager<T>
    {
        [SerializeField]
        private UnityEngine.Camera _mainCamera;

        [SerializeField]
        private LoadingScreen _loadingScreen;

        [Space(10)]

#region Manager Prefabs
        [Header("Manager Prefabs")]

        [SerializeField]
        private PartyParrotManager _engineManagerPrefab;

        [SerializeField]
        private DebugMenuManager _debugMenuManagerPrefab;

        [SerializeField]
        private TimeManager _timeManagerPrefab;

        [SerializeField]
        private SaveGameManager _saveGameManagerPrefab;

        [SerializeField]
        private UIManager _uiManagerPrefab;

        [SerializeField]
        private LocalizationManager _localizationManagerPrefab;

        [SerializeField]
        private AudioManager _audioManagerPrefab;

        [SerializeField]
        private ViewerManager _viewerManagerPrefab;

        [SerializeField]
        private InputManager _inputManagerPrefab;

        [SerializeField]
        private NetworkManager _networkManagerPrefab;

        [SerializeField]
        private SceneManager _sceneManagerPrefab;

        [SerializeField]
        private ObjectPoolManager _objectPoolManagerPrefab;

        [SerializeField]
        private SpawnManager _spawnManagerPrefab;

        [SerializeField]
        private ActorManager _actorManagerPrefab;

        [SerializeField]
        private EffectsManager _effectsManagerPrefab;
#endregion

        [Space(10)]

        [SerializeField]
        [CanBeNull]
        private LoadingTipData _loadingTips;

        [SerializeReference]
        [ReadOnly]
        private ITimer _loadingTipTimer;

        protected GameObject ManagersContainer { get; private set; }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _mainCamera.clearFlags = CameraClearFlags.SolidColor;
            _mainCamera.backgroundColor = Color.black;
            _mainCamera.orthographic = true;
            _mainCamera.useOcclusionCulling = false;

            ShowLoadingScreen(true);

            ManagersContainer = new GameObject("Managers");
        }

        protected virtual void Start()
        {
            StartCoroutine(LoadRoutine());
        }
#endregion

        private IEnumerator LoadRoutine()
        {
            UpdateLoadingScreen(0.0f, "Creating managers...");
            yield return null;

            PreCreateManagers();
            yield return null;

            _loadingTipTimer = TimeManager.Instance.AddTimer();
            _loadingTipTimer.TimesUpEvent += LoadingTimeTimerTimesUpEventHandler;

            ShowNextLoadingTip();
            yield return null;

            CreateManagers();
            yield return null;

            UpdateLoadingScreen(0.25f, "Initializing managers...");
            yield return null;

            IEnumerator<LoadStatus> runner = InitializeManagersRoutine();
            while(runner.MoveNext()) {
                LoadStatus status = runner.Current;
                if(null != status) {
                    UpdateLoadingScreen(0.25f + status.LoadPercent * 0.25f, status.Status);
                }
                yield return null;
            }

            UpdateLoadingScreen(0.75f, "Loading managers...");
            yield return null;

            runner = OnLoadRoutine();
            while(runner.MoveNext()) {
                LoadStatus status = runner.Current;
                if(null != status) {
                    UpdateLoadingScreen(0.75f + status.LoadPercent * 0.25f, status.Status);
                }
                yield return null;
            }

            UpdateLoadingScreen(1.0f, "Manager loading complete!");
            yield return null;

            ShowLoadingScreen(false);
            yield return null;
        }

        private void PreCreateManagers()
        {
            // third party stuff
#if USE_DOTWEEN
            DG.Tweening.DOTween.Init();
#endif

            // core managers
            DebugMenuManager.CreateFromPrefab(_debugMenuManagerPrefab, ManagersContainer);
            TimeManager.CreateFromPrefab(_timeManagerPrefab, ManagersContainer);
            PartyParrotManager.CreateFromPrefab(_engineManagerPrefab, ManagersContainer);
            SaveGameManager.CreateFromPrefab(_saveGameManagerPrefab, ManagersContainer);
            UIManager.CreateFromPrefab(_uiManagerPrefab, ManagersContainer);
            LocalizationManager.CreateFromPrefab(_localizationManagerPrefab, ManagersContainer);

            // TODO: for now this dude does stuff in Start() rather than Awake()
            // someday when Awake() can be overriden, we can get rid of PreCreateManagers()
            // and just do everything in CreateManagers()
            Instantiate(_networkManagerPrefab, ManagersContainer.transform);

            // do this now so that managers coming up can have access to it
            PartyParrotManager.Instance.RegisterLoadingManager(this);
        }

        protected virtual void CreateManagers()
        {
            AudioManager.CreateFromPrefab(_audioManagerPrefab, ManagersContainer);
            EffectsManager.CreateFromPrefab(_effectsManagerPrefab, ManagersContainer);
            ObjectPoolManager.CreateFromPrefab(_objectPoolManagerPrefab, ManagersContainer);
            ActorManager.CreateFromPrefab(_actorManagerPrefab, ManagersContainer);
            ViewerManager.CreateFromPrefab(_viewerManagerPrefab, ManagersContainer);
            InputManager.CreateFromPrefab(_inputManagerPrefab, ManagersContainer);
            SceneManager.CreateFromPrefab(_sceneManagerPrefab, ManagersContainer);
            TerrainManager.Create(ManagersContainer);
            ScriptingManager.Create(ManagersContainer);
            SpawnManager.CreateFromPrefab(_spawnManagerPrefab, ManagersContainer);
        }

        protected virtual IEnumerator<LoadStatus> InitializeManagersRoutine()
        {
            yield break;
        }

        protected virtual IEnumerator<LoadStatus> OnLoadRoutine()
        {
            yield break;
        }

#region Loading Screen
        public void ShowLoadingScreen(bool show)
        {
            _mainCamera.cullingMask = show ? -1 : 0;
            _loadingScreen.gameObject.SetActive(show);

            if(show) {
                ResetLoadingScreen();
            } else {
                _loadingTipTimer.Stop();
            }
        }

        public void ResetLoadingScreen()
        {
            SetLoadingScreenPercent(0.0f);
            SetLoadingScreenText("Loading...");

            ShowNextLoadingTip();
        }

        public void UpdateLoadingScreen(float percent, string text)
        {
            SetLoadingScreenPercent(percent);
            SetLoadingScreenText(text);

            Debug.Log($"{percent * 100}%: {text}");
        }

        public void SetLoadingScreenText(string text)
        {
            _loadingScreen.ProgressText = text;
        }

        public void SetLoadingScreenPercent(float percent)
        {
            _loadingScreen.ProgressBar.Percent = Mathf.Clamp01(percent);
        }

        private void ShowNextLoadingTip()
        {
            if(null == _loadingTips) {
                return;
            }

            _loadingScreen.ShowLoadingTip(_loadingTips.GetRandomLoadingTip());

            _loadingTipTimer.Start(_loadingTips.LoadingTipRotateSeconds);
        }
#endregion

#region Events
        private void LoadingTimeTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            ShowNextLoadingTip();
        }
#endregion
    }
}
