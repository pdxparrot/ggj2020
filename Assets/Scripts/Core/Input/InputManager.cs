using JetBrains.Annotations;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Core.Input
{
    public sealed class InputManager : SingletonBehavior<InputManager>
    {
        // config keys
        private const string EnableVibrationKey = "input.vibration.enable";

        [SerializeField]
        private InputData _inputData;

        public InputData InputData => _inputData;

        [SerializeField]
        private EventSystemHelper _eventSystemPrefab;

        public EventSystemHelper EventSystem { get; private set; }

        public bool EnableVibration
        {
            get => PartyParrotManager.Instance.GetBool(EnableVibrationKey, true);
            set => PartyParrotManager.Instance.SetBool(EnableVibrationKey, value);
        }

#region Debug
        [SerializeField]
        private bool _enableDebug;

        public bool EnableDebug => _enableDebug;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();

            bool inputInitialized = false;

#if ENABLE_VR
            if(!inputInitialized && PartyParrotManager.Instance.EnableVR) {
                Debug.LogError("TODO: Handle VR Input");
                inputInitialized = true;
            }
#endif

#if ENABLE_GVR
            if(!inputInitialized && PartyParrotManager.Instance.EnableGoogleVR) {
                Debug.LogError("TODO: Handle Google VR Input");
                inputInitialized = true;
            }
#endif

            if(!inputInitialized) {
                Debug.Log("Creating EventSystem...");
                EventSystem = Instantiate(_eventSystemPrefab, transform);
            }
        }

        protected override void OnDestroy()
        {
            Destroy(EventSystem.gameObject);
            EventSystem = null;

            base.OnDestroy();
        }
#endregion

        public int GetGamepadCount()
        {
            int count = 0;
            foreach(InputDevice device in InputSystem.devices) {
                if(!(device is Gamepad)) {
                    continue;
                }
                count++;
            }
            return count;
        }

        [CanBeNull]
        public Gamepad GetGamepad(short playerControllerId)
        {
            int gamepadCount = GetGamepadCount();
            if(playerControllerId >= gamepadCount) {
                Debug.LogError($"Player controller {playerControllerId} is greater than available gamepads ({gamepadCount})!");
                return null;
            }

            // TODO: seriously?
            int count = 0;
            foreach(InputDevice device in InputSystem.devices) {
                if(!(device is Gamepad gamepad)) {
                    continue;
                }

                if(count == playerControllerId) {
                    return gamepad;
                }
                count++;
            }

            Debug.LogError($"Unable to find gamepad for player {playerControllerId}");
            return null;
        }

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.InputManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Gamepads", GUI.skin.box);
                    _enableDebug = GUILayout.Toggle(_enableDebug, "Enable Debug");
                    EnableVibration = GUILayout.Toggle(EnableVibration, "Enable Vibration");
                GUILayout.EndVertical();
            };
        }
    }
}

