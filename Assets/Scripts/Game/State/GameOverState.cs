using System;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

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

        [SerializeField]
        private float _completeWaitTimeSeconds = 5.0f;

        [SerializeReference]
        [ReadOnly]
        private ITimer _completeTimer;

        public override void OnEnter()
        {
            base.OnEnter();

            // TODO: show game over UI
        }

        protected override void DoEnter()
        {
            base.DoEnter();

            if(null == _menuPrefab) {
                _completeTimer = TimeManager.Instance.AddTimer();
                _completeTimer.TimesUpEvent += CompleteTimerTimesUpEventHandler;
                _completeTimer.Start(_completeWaitTimeSeconds);
            } else {
                _menu = GameStateManager.Instance.GameUIManager.InstantiateUIPrefab(_menuPrefab);
                _menu.Initialize();
            }
        }

        protected override void DoExit()
        {
            if(null == _menu) {
                TimeManager.Instance.RemoveTimer(_completeTimer);
                _completeTimer = null;
            } else {
                Destroy(_menu.gameObject);
                _menu = null;
            }

            AudioManager.Instance.StopAllMusic();

            base.DoExit();
        }

        public virtual void Initialize()
        {
        }

#region Event Handlers
        private void CompleteTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            GameStateManager.Instance.TransitionToInitialStateAsync();
        }
#endregion
    }
}
