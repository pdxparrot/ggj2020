using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    [RequireComponent(typeof(PlayerInputHelper))]
    public abstract class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        public Actor Owner => _owner;

        protected IPlayer Player => (IPlayer)Owner;

        [SerializeField]
        private PlayerInputData _data;

        public PlayerInputData PlayerInputData => _data;

        [SerializeField]
        private float _mouseSensitivity = 0.5f;

        protected float MouseSensitivity => _mouseSensitivity;

#region Input Buffers
        [SerializeField]
        [ReadOnly]
        private long _lastMoveBufferTimestampMs;

        private bool MoveBufferExpired => TimeManager.Instance.CurrentUnixMs - _lastMoveBufferTimestampMs > PlayerInputData.InputBufferTimeoutMs;

        private CircularBuffer<Vector3> _moveBuffer;

        public Vector3 LastMove => _moveBuffer.Tail;

        [SerializeField]
        [ReadOnly]
        private long _lastLookBufferTimestampMs;

        private bool LookBufferExpired => TimeManager.Instance.CurrentUnixMs - _lastLookBufferTimestampMs > PlayerInputData.InputBufferTimeoutMs;

        private CircularBuffer<Vector3> _lookBuffer;

        public Vector3 LastLook => _moveBuffer.Tail;
#endregion

        protected virtual bool InputEnabled => !PartyParrotManager.Instance.IsPaused && Player.IsLocalActor  && GameStateManager.Instance.GameManager.IsGameReady && !GameStateManager.Instance.GameManager.IsGameOver;

        protected bool EnableMouseLook { get; private set; } = !Application.isEditor;

        public PlayerInputHelper InputHelper { get; private set; }

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsTrue(Owner is IPlayer);
            Assert.IsNotNull(PlayerInputData);
            Assert.IsTrue(PlayerInputData.InputBufferSize > 0);
            Assert.IsTrue(PlayerInputData.InputBufferTimeoutMs > 0);

            InputHelper = GetComponent<PlayerInputHelper>();

            _moveBuffer = new CircularBuffer<Vector3>(PlayerInputData.InputBufferSize);
            _lookBuffer = new CircularBuffer<Vector3>(PlayerInputData.InputBufferSize);
        }

        protected virtual void OnDestroy()
        {
            DestroyDebugMenu();
        }

        protected virtual void Update()
        {
            if(!Player.IsLocalActor) {
                return;
            }

            UpdateBuffers();
        }
#endregion

        public virtual void Initialize(short playerControllerId)
        {
            if(!Player.IsLocalActor) {
                return;
            }

            InputHelper.Initialize(playerControllerId);

            InitDebugMenu();
        }

        // TODO: fix the naming conflict between these
        protected bool InputAllowed => InputEnabled && Application.isFocused;

        protected virtual bool IsInputAllowed(InputAction.CallbackContext ctx)
        {
            if(!InputAllowed) {
                return false;
            }

            // ignore keyboard/mouse while the debug menu is open
            if(DebugMenuManager.Instance.Enabled && (ctx.control.device == Keyboard.current || ctx.control.device == Mouse.current)) {
                return false;
            }

            return true;
        }

        private void UpdateBuffers()
        {
            if(_moveBuffer.Count > 0 && MoveBufferExpired) {
                _moveBuffer.RemoveOldest();
            }

            if(_lookBuffer.Count > 0 && LookBufferExpired) {
                _lookBuffer.RemoveOldest();
            }
        }

#region Common Actions
        public void OnPause()
        {
            PartyParrotManager.Instance.TogglePause();
        }

        public void OnMove(Vector3 axes)
        {
            _moveBuffer.Add(axes);
            _lastMoveBufferTimestampMs = TimeManager.Instance.CurrentUnixMs;
        }

        public void OnLook(Vector3 axes)
        {
            _lookBuffer.Add(axes);
            _lastLookBufferTimestampMs = TimeManager.Instance.CurrentUnixMs;
        }
#endregion

#region Debug Menu
        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => $"Game.Player {Player.Id} Input");
            _debugMenuNode.RenderContentsAction = () => {
                /*GUILayout.BeginHorizontal();
                    GUILayout.Label("Mouse Sensitivity:");
                    _mouseSensitivity = GUIUtils.FloatField(_mouseSensitivity);
                GUILayout.EndHorizontal();*/

                if(Application.isEditor) {
                    EnableMouseLook = GUILayout.Toggle(EnableMouseLook, "Enable Mouse Look");
                }

                GUILayout.BeginVertical("Move Buffer", GUI.skin.box);
                    foreach(var move in _moveBuffer) {
                        GUILayout.Label(move.ToString());
                    }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("Look Buffer", GUI.skin.box);
                    foreach(var look in _lookBuffer) {
                        GUILayout.Label(look.ToString());
                    }
                GUILayout.EndVertical();
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
#endregion
    }
}
