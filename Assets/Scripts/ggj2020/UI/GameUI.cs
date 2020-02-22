using pdxpartyparrot.Core.Effects;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public sealed class GameUI : Game.UI.GameUI
    {
        [SerializeField]
        private PlayerHUD _hud;

        public PlayerHUD HUD => _hud;

#region Intro UI
        [SerializeField]
        private GameObject _introUI;

        [SerializeField]
        private GameObject _useChargingStationUI;

        [SerializeField]
        private GameObject _noUseChargingStationUI;

        [SerializeField]
        private EffectTrigger _showIntroUIEffectTrigger;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _introUI.gameObject.SetActive(false);
        }
#endregion

        public void EnableChargingStationIntroUI(bool enable)
        {
            _useChargingStationUI.gameObject.SetActive(enable);
            _noUseChargingStationUI.gameObject.SetActive(!enable);
        }

        public void ShowIntroUI()
        {
            _showIntroUIEffectTrigger.Trigger();
        }
    }
}
