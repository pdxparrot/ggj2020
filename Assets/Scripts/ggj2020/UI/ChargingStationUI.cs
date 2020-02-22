using System.Collections;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public sealed class ChargingStationUI : MonoBehaviour
    {
        public enum EnabledUI
        {
            Robot,
            Battery,
            PlayerInteract,
        }

        [SerializeField]
        private float _animateTime = 1.0f;

        [SerializeField]
        private GameObject _robotIcon;

        [SerializeField]
        private GameObject _batteryIcon;

        [SerializeField]
        private GameObject _playerInteractUI;

        [SerializeField]
        [ReadOnly]
        private EnabledUI _enabledUI = EnabledUI.Robot;

#region Unity Lifecycle
        private void Awake()
        {
            StartCoroutine(AnimateRoutine());
        }
#endregion

        public void SetUI(EnabledUI enabledUI)
        {
            _enabledUI = enabledUI;
            switch(_enabledUI)
            {
            case EnabledUI.PlayerInteract:
                _robotIcon.SetActive(false);
                _batteryIcon.SetActive(false);
                _playerInteractUI.SetActive(true);
                break;
            case EnabledUI.Robot:
                _robotIcon.SetActive(true);
                _batteryIcon.SetActive(false);
                _playerInteractUI.SetActive(false);
                break;
            case EnabledUI.Battery:
                _robotIcon.SetActive(false);
                _batteryIcon.SetActive(true);
                _playerInteractUI.SetActive(false);
                break;
            }
        }

        private IEnumerator AnimateRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_animateTime);
            while(true) {
                yield return wait;

                switch(_enabledUI)
                {
                case EnabledUI.PlayerInteract:
                    break;
                case EnabledUI.Robot:
                    SetUI(EnabledUI.Battery);
                    break;
                case EnabledUI.Battery:
                    SetUI(EnabledUI.Robot);
                    break;
                }
            }
        }
    }
}
