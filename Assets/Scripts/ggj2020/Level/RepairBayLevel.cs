using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Game.Level;
using pdxpartyparrot.ggj2020.Actors;

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

        private RepairableRobot _repairableRobot;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _timer = TimeManager.Instance.AddTimer();
            _timer.TimesUpEvent += OnRepairTimeUp;
        }

        protected override void OnDestroy()
        {
            Destroy(_repairableRobot);

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_timer);
                _timer = null;
            }

            base.OnDestroy();
        }
#endregion

#region Events
        protected override void GameStartServerEventHandler(object sender, EventArgs args)
        {
            base.GameStartServerEventHandler(sender, args);

            _repairableRobot = Instantiate(GameManager.Instance.GameGameData.RepairableRobotPrefab, transform);
            _repairableRobot.gameObject.SetActive(false);
        }

        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }

        protected override void GameReadyEventHandler(object sender, EventArgs args)
        {
            _repairableRobot.gameObject.SetActive(true);
            _repairableRobot.EnterRepairBay();

            _timer.Start(GameManager.Instance.GameGameData.RepairTime);
        }

        private void OnRepairTimeUp(object sender, EventArgs args)
        {
            _repairableRobot.ExitRepairBay();

            // TODO: advance to the next robot

            // TODO: when does the game end?
        }
#endregion
    }
}
