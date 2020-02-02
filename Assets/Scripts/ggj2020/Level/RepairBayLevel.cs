using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.World;
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

        [SerializeField]
        private Transform _repairableExit;

        public Transform RepairableExit => _repairableExit;

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
            _repairableRobot.RepairedEvent -= RepairedEventHandler;
            Destroy(_repairableRobot);

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_timer);
                _timer = null;
            }

            base.OnDestroy();
        }
#endregion

        private void NextRobot()
        {
            GameManager.Instance.MechanicsCanInteract = false;

            // TODO: when does the game end?

            _repairableRobot.ExitRepairBay(() => {
                // TODO: advance to the next robot
            });

            // TODO: kick off the next background battle
        }

#region Events
        protected override void GameStartServerEventHandler(object sender, EventArgs args)
        {
            base.GameStartServerEventHandler(sender, args);

            SpawnPoint spawnpoint = SpawnManager.Instance.GetSpawnPoint(GameManager.Instance.GameGameData.RepairableRobotSpawnTag);
            _repairableRobot = spawnpoint.SpawnNPCPrefab(GameManager.Instance.GameGameData.RepairableRobotPrefab, null, transform).GetComponent<RepairableRobot>();
            _repairableRobot.RepairedEvent += RepairedEventHandler;
        }

        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }

        protected override void GameReadyEventHandler(object sender, EventArgs args)
        {
            GameManager.Instance.MechanicsCanInteract = false;

            // this will init the timer UI correctly
            _timer.AddTime(GameManager.Instance.GameGameData.RepairTime);

            _repairableRobot.gameObject.SetActive(true);
            _repairableRobot.EnterRepairBay(() => {
                GameManager.Instance.MechanicsCanInteract = true;

                _timer.Start(GameManager.Instance.GameGameData.RepairTime);
            });
        }

        private void OnRepairTimeUp(object sender, EventArgs args)
        {
            Debug.Log("Times up!");
            NextRobot();
        }

        private void RepairedEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Robot repaired!");
            NextRobot();
        }
#endregion
    }
}
