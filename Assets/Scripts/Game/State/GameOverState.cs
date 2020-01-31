using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Input;

using UnityEngine;

namespace pdxpartyparrot.Game.State
{
    // TODO: we probably need a TransitionToInitialState effect trigger component
    // now that this can no longer automatically transition on its own

    public abstract class GameOverState : SubGameState
    {
        [SerializeField]
        private Menu.Menu _menuPrefab;

        private Menu.Menu _menu;

        public override void OnEnter()
        {
            base.OnEnter();

            // TODO: show game over text
        }

        protected override void DoEnter()
        {
            base.DoEnter();

            if(null != _menuPrefab) {
                InputManager.Instance.EventSystem.UIModule.EnableAllActions();

                _menu = GameStateManager.Instance.GameUIManager.InstantiateUIPrefab(_menuPrefab);
                _menu.Initialize();
            }
        }

        protected override void DoExit()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.DisableAllActions();
            }

            if(null != _menu) {
                Destroy(_menu.gameObject);
                _menu = null;
            }

            AudioManager.Instance.StopAllMusic();

            base.DoExit();
        }

        public virtual void Initialize()
        {
        }
    }
}
