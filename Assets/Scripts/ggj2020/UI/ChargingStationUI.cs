using System.Collections;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.UI;

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
        private GameObject _batteryChargingIcon;

        [SerializeField]
        private Image _batteryChargeLevel;

        [SerializeField]
        private GameObject _nextPlayerIcon;

        [SerializeField]
        [ReadOnly]
        private EnabledUI _enabledUI = EnabledUI.Robot;

        [SerializeField]
        [ReadOnly]
        private float _displayCharge;

        private Coroutine _animateRoutine;

#region Unity Lifecycle
        private void OnEnable()
        {
            _animateRoutine = StartCoroutine(AnimateRoutine());
        }

        private void OnDisable()
        {
            StopCoroutine(_animateRoutine);
            _animateRoutine = null;
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            UpdatePlayerInteractUI(dt);
        }
#endregion

        public void UpdateCharge()
        {
            _displayCharge = GameManager.Instance.GameLevelHelper.ChargingStation.ChargePercent;

            UpdateChargeUI();
        }

        private void UpdatePlayerInteractUI(float dt)
        {
            if(EnabledUI.PlayerInteract != _enabledUI) {
                return;
            }

            _displayCharge = Mathf.MoveTowards(_displayCharge, 1.0f, dt * _animateTime);
            if(Mathf.Approximately(_displayCharge, 1.0f)) {
                _displayCharge = GameManager.Instance.GameLevelHelper.ChargingStation.ChargePercent;
            }

            UpdateChargeUI();
        }

        private void UpdateChargeUI()
        {
            _batteryChargeLevel.fillAmount = _displayCharge;
        }

        public void SetUI(EnabledUI enabledUI)
        {
            bool charged = GameManager.Instance.GameLevelHelper.ChargingStation.IsCharged;
            bool charging = GameManager.Instance.GameLevelHelper.ChargingStation.IsCharging;

            UpdateCharge();

            _enabledUI = enabledUI;
            switch(_enabledUI)
            {
            case EnabledUI.PlayerInteract:
                _robotIcon.SetActive(false);
                _nextPlayerIcon.SetActive(false);

                _batteryIcon.SetActive(false);
                _batteryChargingIcon.SetActive(true);
                break;
            case EnabledUI.Robot:
                _robotIcon.SetActive(!charged && !charging);
                _nextPlayerIcon.SetActive(!charged && charging);

                _batteryIcon.SetActive(false);
                _batteryChargingIcon.SetActive(charged);
                break;
            case EnabledUI.Battery:
                _robotIcon.SetActive(false);
                _nextPlayerIcon.SetActive(false);

                _batteryIcon.SetActive(!charged && !charging);
                _batteryChargingIcon.SetActive(charged || charging);
                break;
            }
        }

        // TODO: instead of using a coroutine
        // a timer that triggers off SetUI would make more sense
        // because we get weird animation timings with a constant routine
        private IEnumerator AnimateRoutine()
        {
            WaitForSeconds wait = new WaitForSeconds(_animateTime);
            while(true) {
                yield return wait;

                switch(_enabledUI)
                {
                case EnabledUI.PlayerInteract:
                    // don't swap animations while the player is interacting
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
