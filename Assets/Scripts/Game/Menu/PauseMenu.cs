using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class PauseMenu : MenuPanel
    {
#region Settings
        [SerializeField]
        private SettingsMenu _settingsMenu;
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _settingsMenu.gameObject.SetActive(false);
        }
#endregion

#region Event Handlers
        public void OnSettings()
        {
            Owner.PushPanel(_settingsMenu);
        }

        public void OnResume()
        {
            OnBack();
        }

        public override void OnBack()
        {
            PartyParrotManager.Instance.TogglePause();
        }

        public void OnExitMainMenu()
        {
            // stop all audio so when it unducks it doesn't blast all weird
            AudioManager.Instance.StopAllAudio();

            GameStateManager.Instance.TransitionToInitialStateAsync();

            PartyParrotManager.Instance.TogglePause();
        }

        public void OnQuitGame()
        {
            UnityUtil.Quit();
        }
#endregion
    }
}
