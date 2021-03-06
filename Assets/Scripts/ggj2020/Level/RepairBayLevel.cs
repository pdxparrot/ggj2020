using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Level;
using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Actors.Tools;
using pdxpartyparrot.ggj2020.Players;
using pdxpartyparrot.ggj2020.UI;

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

        public ChargingStation ChargingStation => _chargingStation;

        [Space(10)]

        [SerializeField]
        private BackgroundRobot _backgroundRobot;

        [Space(10)]

        [SerializeField]
        private Collider2D _cameraBounds;

        [SerializeField]
        private Transform _repairableExit;

        public Transform RepairableExit => _repairableExit;

        [SerializeField]
        [ReadOnly]
        private int _currentRound;

        [SerializeReference]
        [ReadOnly]
        private ITimer _timer;

        [SerializeReference]
        [ReadOnly]
        private ITimer _respawnTimer;

        public float TimeRemaining => (int)_timer.SecondsRemaining;

        [CanBeNull]
        private RepairableRobot _repairableRobot;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _timer = TimeManager.Instance.AddTimer();
            _timer.TimesUpEvent += RepairTimesUpEventHandler;

            _respawnTimer = TimeManager.Instance.AddTimer();
            _respawnTimer.TimesUpEvent += RespawnRobotEventHandler;

            _chargingStation.ChargeCompleteEvent += ChargeCompleteEventHandler;
        }

        protected override void OnDestroy()
        {
            _chargingStation.ChargeCompleteEvent -= ChargeCompleteEventHandler;

            if(null != _repairableRobot) {
                _repairableRobot.RepairedEvent -= RepairedEventHandler;
                Destroy(_repairableRobot);
            }
            _repairableRobot = null;

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

            _chargingStation.EnableUI(false);

            _repairableRobot.ExitRepairBay(success, () => {
                _repairableRobot.DeSpawn();

                _backgroundRobot.BeginFight(success, _chargingStation.IsCharged);

                _respawnTimer.Start(GameManager.Instance.GameGameData.RobotRespawnRate);
            });
        }

        private void EnterRobot()
        {
            float roundTime = GameManager.Instance.GameGameData.RepairTime;
            switch(_currentRound)
            {
            case 0:
                roundTime *= 3.0f;
                break;
            case 1:
                roundTime *= 2.0f;
                break;
            case 2:
                roundTime *= 1.5f;
                break;
            }
            _currentRound++;

            // this will init the timer UI correctly
            _timer.SecondsRemaining = roundTime;

            _chargingStation.ResetCharge();

            _repairableRobot.EnterRepairBay(() => {
                _chargingStation.EnableUI(true);

                GameManager.Instance.MechanicsCanInteract = true;

                _timer.Start(roundTime);
            });
        }

        private void FullyRepaired()
        {
            _timer.Stop();

            GameManager.Instance.RepairSuccess(1.0f);

            NextRobot(true);
        }

#region Events
        protected override void GameStartServerEventHandler(object sender, EventArgs args)
        {
            base.GameStartServerEventHandler(sender, args);

            GameManager.Instance.MechanicsCanInteract = false;
        }

        protected override void GameStartClientEventHandler(object sender, EventArgs args)
        {
            base.GameStartClientEventHandler(sender, args);

            bool enableChargingStation = PlayerManager.Instance.PlayerCount >= GameManager.Instance.GameGameData.ChargingStationMinPlayers;

            _chargingStation.Enable(enableChargingStation);
            _chargingStation.EnableUI(false);

            GameManager.Instance.Viewer.SetBounds(_cameraBounds);

            GameUIManager.Instance.GameGameUI.EnableChargingStationIntroUI(enableChargingStation);

            GameManager.Instance.IntroCompleteEvent += IntroCompleteEventHandler;
        }

        private void IntroCompleteEventHandler(object sender, EventArgs args)
        {
            GameManager.Instance.IntroCompleteEvent -= IntroCompleteEventHandler;

            SpawnPoint spawnpoint = SpawnManager.Instance.GetSpawnPoint(GameManager.Instance.GameGameData.RepairableRobotSpawnTag);
            _repairableRobot = spawnpoint.SpawnNPCPrefab(GameManager.Instance.GameGameData.RepairableRobotPrefab, null, transform).GetComponent<RepairableRobot>();
            _repairableRobot.RepairedEvent += RepairedEventHandler;

            EnterRobot();
        }

        private void RepairTimesUpEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Times up!");

            float repairPercent = _repairableRobot.GetRepairPercent();
            bool success = _chargingStation.IsCharged && repairPercent >= GameManager.Instance.GameGameData.PassingRepairPercent;
            if(!success) {
                if(!GameManager.Instance.RepairFailure(_chargingStation.IsCharged, repairPercent)) {
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

        private void ChargeCompleteEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Robot fully charged!");

            if(!_repairableRobot.IsRepaired()) {
                Debug.Log("Robot waiting to be repaired...");
                return;
            }

            FullyRepaired();
        }

        private void RepairedEventHandler(object sender, EventArgs args)
        {
            Debug.Log("Robot fully repaired!");

            if(!_chargingStation.IsCharged) {
                Debug.Log("Robot waiting to be charged...");
                return;
            }

            FullyRepaired();
        }
#endregion
    }
}
