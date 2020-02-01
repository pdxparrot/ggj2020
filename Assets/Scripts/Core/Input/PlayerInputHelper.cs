using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Core.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class PlayerInputHelper : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private bool _isRumbling;

        private PlayerInput _playerInput;

#region Unity Lifecycle
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnDestroy()
        {
            // make sure we don't leave the gamepad rumbling
            foreach(InputDevice device in _playerInput.devices) {
                if(!(device is Gamepad gamepad)) {
                    continue;
                }

                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
        }
#endregion

        public void EnableControls(bool enable)
        {
            if(enable) {
                _playerInput.actions.Enable();
            } else {
                _playerInput.actions.Disable();
            }
        }

        public void Rumble(RumbleConfig config)
        {
            if(!InputManager.Instance.EnableVibration || _isRumbling) {
                return;
            }

            if(InputManager.Instance.EnableDebug) {
                Debug.Log($"Rumbling player input {_playerInput.playerIndex} for {config.Seconds} seconds, (low: {config.LowFrequency} high: {config.HighFrequency})");
            }

            foreach(InputDevice device in _playerInput.devices) {
                if(!(device is Gamepad gamepad)) {
                    continue;
                }

                gamepad.SetMotorSpeeds(config.LowFrequency, config.HighFrequency);
                _isRumbling = true;

                TimeManager.Instance.RunAfterDelay(config.Seconds, () => {
                    gamepad.SetMotorSpeeds(0.0f, 0.0f);
                    _isRumbling = false;
                });
            }
        }
    }
}
