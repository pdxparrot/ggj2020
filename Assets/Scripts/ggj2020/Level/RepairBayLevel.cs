using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Game.Level;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Level
{
    public class RepairBayLevel : LevelHelper
    {
        [Space(10)]

        [SerializeField]
        private Collider2D _cameraBounds;

        private ITimer _timer;

        public float TimeRemaining => (int)_timer.SecondsRemaining;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _timer = TimeManager.Instance.AddTimer();
            _timer.TimesUpEvent += OnRepairTimeUp;
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_timer);
                _timer = null;
            }

            base.OnDestroy();
        }
#endregion

#region Events
        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }

        protected override void GameReadyEventHandler(object sender, EventArgs args)
        {
            // TODO: animate in the first robot

            _timer.Start(GameManager.Instance.GameGameData.RepairTime);
        }

        private void OnRepairTimeUp(object sender, EventArgs args)
        {
            // TODO: advance to the next robot

            // TODO: when does the game end?
        }
#endregion
    }
}
