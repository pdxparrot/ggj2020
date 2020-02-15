using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Level;
using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.Tools;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Level
{
    public sealed class RepairBayLevel : LevelHelper
    {
        [Space(10)]

        [SerializeField]
        private Transform _toolContainer;

        [SerializeField]
        private ChargingStation _chargingStation;

        [Space(10)]

        [SerializeField]
        private Collider2D _cameraBounds;

        [SerializeField]
        private Transform _repairableExit;

        public Transform RepairableExit => _repairableExit;

        private ITimer _timer;

        private ITimer _respawnTimer;

        public float TimeRemaining => (int)_timer.SecondsRemaining;

        private RepairableRobot _repairableRobot;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _timer = TimeManager.Instance.AddTimer();
            _timer.TimesUpEvent += RepairTimesUpEventHandler;

            _respawnTimer = TimeManager.Instance.AddTimer();
            _respawnTimer.TimesUpEvent += RespawnRobotEventHandler;
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

        public void ReclaimTool(Tool tool)
        {
            tool.transform.SetParent(_toolContainer);
        }

        private void NextRobot(bool success)
        {
            GameManager.Instance.MechanicsCanInteract = false;

            _repairableRobot.ExitRepairBay(success, () => {
                _repairableRobot.DeSpawn();

                // TODO: kick off the next background battle

                // TODO: really this should go after the background battle finishes or is close to finishing
                _respawnTimer.Start(GameManager.Instance.GameGameData.RobotRespawnRate);
            });
        }

        private void EnterRobot()
        {
            // this will init the timer UI correctly
            _timer.SecondsRemaining = GameManager.Instance.GameGameData.RepairTime;

            _repairableRobot.EnterRepairBay(() => {
                GameManager.Instance.MechanicsCanInteract = true;

                _timer.Start(GameManager.Instance.GameGameData.RepairTime);
            });
        }

#region Events
        protected override void GameStartServerEventHandler(object sender, EventArgs args)
        {
            base.GameStartServerEventHandler(sender, args);

            _chargingStation.gameObject.SetActive(PlayerManager.Instance.Players.Count >= GameManager.Instance.GameGameData.ChargingStationMinPlayers);

            GameManager.Instance.MechanicsCanInteract = false;
        }

        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }

        protected override void GameReadyEventHandler(object sender, EventArgs args)
        {
            SpawnPoint spawnpoint = SpawnManager.Instance.GetSpawnPoint(GameManager.Instance.GameGameData.RepairableRobotSpawnTag);
            _repairableRobot = spawnpoint.SpawnNPCPrefab(GameManager.Instance.GameGameData.RepairableRobotPrefab, null, transform).GetComponent<RepairableRobot>();
            _repairableRobot.RepairedEvent += RepairedEventHandler;

            EnterRobot();
        }

        private void RepairTimesUpEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Times up!");

            float repairPercent = _repairableRobot.GetRepairPercent();
            bool success = repairPercent >= GameManager.Instance.GameGameData.PassingRepairPercent;
            if(!success) {
                if(!GameManager.Instance.RepairFailure(repairPercent)) {
                    return;
                }
            } else {
                GameManager.Instance.RepairSuccess(repairPercent);
            }

            NextRobot(success);
        }

        private void RespawnRobotEventHandler(object sender, EventArgs args)
        {
            SpawnPoint spawnpoint = SpawnManager.Instance.GetSpawnPoint(GameManager.Instance.GameGameData.RepairableRobotSpawnTag);
            spawnpoint.ReSpawn(_repairableRobot);

            EnterRobot();
        }

        private void RepairedEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Robot fully repaired!");

            _timer.Stop();

            GameManager.Instance.RepairSuccess(1.0f);

            NextRobot(true);
        }
#endregion
    }
}
