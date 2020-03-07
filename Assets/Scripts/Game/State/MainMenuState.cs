using JetBrains.Annotations;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.Game.UI;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    public class MainMenuState : GameState
    {
        [SerializeField]
        private Menu.Menu _menuPrefab;

        [CanBeNull]
        protected Menu.Menu Menu { get; private set; }

        [CanBeNull]
        protected MainMenu MainMenu => (MainMenu)Menu.MainPanel;

        [SerializeField]
        private TitleScreen _titleScreenPrefab;

        [CanBeNull]
        protected TitleScreen TitleScreen { get; private set; }

        public override void OnEnter()
        {
            base.OnEnter();

            Menu = GameStateManager.Instance.GameUIManager.InstantiateUIPrefab(_menuPrefab);
            Menu.Initialize();

            TitleScreen = GameStateManager.Instance.GameUIManager.InstantiateUIPrefab(_titleScreenPrefab);

            if(GameStateManager.Instance.GameManager.TransitionToHighScores && null != MainMenu) {
                TitleScreen.FinishLoading();
                MainMenu.ShowHighScores();
            }
        }

        protected override void DoExit()
        {
            if(AudioManager.HasInstance) {
                AudioManager.Instance.StopAllMusic();
            }

            if(null != TitleScreen) {
                Destroy(TitleScreen.gameObject);
            }
            TitleScreen = null;

            if(null != Menu) {
                Destroy(Menu.gameObject);
            }
            Menu = null;
        }

        public override void OnResume()
        {
            base.OnResume();

            Menu.gameObject.SetActive(true);
        }

        public override void OnPause()
        {
            Menu.gameObject.SetActive(false);

            base.OnPause();
        }
    }
}
