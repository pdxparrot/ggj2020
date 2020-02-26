using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

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
        private GameObject _introIntroUI;

        [SerializeField]
        [FormerlySerializedAs("_showIntroUIEffectTrigger")]
        private EffectTrigger _showOldIntroUIEffectTrigger;

        [SerializeField]
        private GameObject[] _introPanels;

        [SerializeField]
        private GameObject _introBackButton;

        [SerializeField]
        [ReadOnly]
        private int _currentIntroSlide;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_introPanels.Length > 0);

            HideIntroUI();
        }
#endregion

        public void EnableChargingStationIntroUI(bool enable)
        {
            /*_useChargingStationUI.gameObject.SetActive(enable);
            _noUseChargingStationUI.gameObject.SetActive(!enable);*/
        }

        public void ShowIntroUI()
        {
            //_showOldIntroUIEffectTrigger.Trigger();

            _introUI.gameObject.SetActive(true);
            _introIntroUI.gameObject.SetActive(true);

            _currentIntroSlide = 0;
            _introPanels[_currentIntroSlide].gameObject.SetActive(true);
        }

        public void HideIntroUI()
        {
            _introUI.gameObject.SetActive(false);

            // old-style intro
            _useChargingStationUI.SetActive(false);
            _noUseChargingStationUI.SetActive(false);

            // new-style intro
            _introIntroUI.gameObject.SetActive(false);
            foreach(GameObject go in _introPanels) {
                go.SetActive(false);
            }
            _introBackButton.gameObject.SetActive(false);
        }

        public bool IntroAdvance()
        {
            _introPanels[_currentIntroSlide].gameObject.SetActive(false);

            _currentIntroSlide++;
            if(_currentIntroSlide >= _introPanels.Length) {
                return true;
            }

            _introPanels[_currentIntroSlide].gameObject.SetActive(true);
            _introBackButton.gameObject.SetActive(true);

            return false;
        }

        public void IntroBack()
        {
            if(0 == _currentIntroSlide) {
                return;
            }

            _introPanels[_currentIntroSlide].gameObject.SetActive(false);

            _currentIntroSlide--;

            _introPanels[_currentIntroSlide].gameObject.SetActive(true);
            _introBackButton.gameObject.SetActive(_currentIntroSlide > 0);
        }
    }
}
