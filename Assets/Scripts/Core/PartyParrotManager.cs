using System;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Rendering;

namespace pdxpartyparrot.Core
{
    public sealed class PartyParrotManager : SingletonBehavior<PartyParrotManager>
    {
        private const string ConfigFileName = "config.json";

#region Events
        public event EventHandler<EventArgs> PauseEvent;
#endregion

        [SerializeField]
        private EngineData _data;

// TODO: a lot of this junk should move to the data object

        [Space(10)]

#region VR Config

#if ENABLE_VR
        [SerializeField]
        private bool _enableVR;

        public bool EnableVR => _enableVR;
#endif

#if ENABLE_GVR
        [SerializeField]
        private bool _enableGoogleVR;

        public bool EnableGoogleVR => _enableGoogleVR;
#endif

#endregion

        [Space(10)]

#region Physics
        [Header("Physics")]

        [SerializeField]
        private PhysicMaterial _frictionlesssMaterial;

        public PhysicMaterial FrictionlessMaterial => _frictionlesssMaterial;

        [SerializeField]
        private PhysicsMaterial2D _frictionlesssMaterial2D;

        public PhysicsMaterial2D FrictionlessMaterial2D => _frictionlesssMaterial2D;
#endregion

        [Space(10)]

// TODO: this should be network sync'd and server-authoritative
#region Game State
        [Header("Game State")]

        [SerializeField]
        [Tooltip("Set to a value greater than 0 to seed the random generator")]
        private int _randomSeed;

        public int RandomSeed => _randomSeed;

        [SerializeField]
        [ReadOnly]
        private bool _isPaused;

        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                bool wasPaused = _isPaused;
                _isPaused = value;

                if(wasPaused != _isPaused) {
                    Debug.Log($"Pause: {_isPaused}");

                    PauseEvent?.Invoke(this, EventArgs.Empty);
                }
            }
        }
#endregion

        [Space(10)]

#region Managers
        [Header("Mangers")]

        [SerializeField]
        [ReadOnly]
        private ILoadingManager _loadingManager;

        public ILoadingManager LoadingManager => _loadingManager;
#endregion

        public bool IsHeadless => GraphicsDeviceType.Null == SystemInfo.graphicsDeviceType;

        public Config Config { get; } = new Config();

        public System.Random Random { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            // TODO: is this the best place for this log?
            Debug.Log("Party Parrot Engine starting up...");
            Debug.Log($"Unity version: {Application.unityVersion}");

            // Environment.TickCount is what C# uses under the hood
            // we duplicate it here so we can save it out (since it's not exposed anywhere)
            SetRandomSeed(_randomSeed > 0 ? _randomSeed : Environment.TickCount);

            if(_data.EnableVSync) {
                Debug.Log("Enabling VSync");

                QualitySettings.vSyncCount = 1;
                Application.targetFrameRate = -1;
            } else {
                Debug.Log($"Disabling VSync, target frame rate: {_data.TargetFrameRate}");

                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = _data.TargetFrameRate;
            }

            Debug.Log($"Gravity: {Physics.gravity}");

            Config.Load(Application.streamingAssetsPath, ConfigFileName);
        }
#endregion

        public void SetRandomSeed(int seed)
        {
            _randomSeed = seed;
            Random = new System.Random(_randomSeed);
        }

#region Manager Registration
        public void RegisterLoadingManager(ILoadingManager loadingManager)
        {
            _loadingManager = loadingManager;
        }
#endregion

        // TODO: PlayerPrefs is bullshit, use a config file
#region Player Config

#region Bool
        public bool GetBool(string key)
        {
            return GetInt(key) != 0;
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return GetInt(key, defaultValue ? 1 : 0) != 0;
        }

        public void SetBool(string key, bool value)
        {
            SetInt(key, value ? 1 : 0);
        }
#endregion

#region Int
        public int GetInt(string key)
        {
            return PlayerPrefs.GetInt(key);
        }

        public int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
        }
#endregion

#region Float
        public float GetFloat(string key)
        {
            return PlayerPrefs.GetFloat(key);
        }

        public float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
        }
#endregion

#region String
        public string GetString(string key)
        {
            return PlayerPrefs.GetString(key);
        }

        public string GetString(string key, string defaultValue)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
        }
#endregion

#region Generic Objects
        public T Get<T>(string key) where T: class
        {
            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        }

        public T Get<T>(string key, T defaultValue) where T: class
        {
            return PlayerPrefs.HasKey(key) ? JsonUtility.FromJson<T>(PlayerPrefs.GetString(key)) : defaultValue;
        }

        public void Set<T>(string key, T value) where T: class
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(value));
        }
#endregion

#endregion

        public void TogglePause()
        {
            IsPaused = !IsPaused;
        }
    }
}
