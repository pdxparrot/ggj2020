using System;
using System.Collections.Generic;

using pdxpartyparrot.ggj2020.Actors;
using pdxpartyparrot.ggj2020.Camera;
using pdxpartyparrot.ggj2020.Data.Actors;
using pdxpartyparrot.ggj2020.State;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Data
{
    [CreateAssetMenu(fileName="GameData", menuName="pdxpartyparrot/ggj2020/Data/Game Data")]
    [Serializable]
    public sealed class GameData : Game.Data.GameData
    {
#region Game States
        [Header("Game States")]

        [SerializeField]
        private MainGameState _mainGameStatePrefab;

        public MainGameState MainGameStatePrefab => _mainGameStatePrefab;
#endregion

        [Space(10)]

#region Viewers
        [Header("Viewer")]

        [SerializeField]
        private GameViewer _viewerPrefab;

        public GameViewer ViewerPrefab => _viewerPrefab;
#endregion

        [Space(10)]

#region Repairable Robot
        [Header("Repairable Robot")]

        [SerializeField]
        private RepairableRobot _repairableRobotPrefab;

        public RepairableRobot RepairableRobotPrefab => _repairableRobotPrefab;

        [SerializeField]
        private RepairableRobotData _repairableRobotData;

        public RepairableRobotData RepairableRobotData => _repairableRobotData;
#endregion

        [Space(10)]

#region Tools
        [Header("Tools")]

        [SerializeField]
        [Range(1, 5)]
        [Tooltip("The minimum number of players required for the charging station to be enabled")]
        private int _chargingStationMinPlayers = 3;

        public int ChargingStationMinPlayers => _chargingStationMinPlayers;

        [SerializeField]
        private float _fireExtinguisherHoldTime = 1;

        public float FireExtinguisherHoldTime => _fireExtinguisherHoldTime;

        [SerializeField]
        private float _hammerCooldown = 0.5f;

        public float HammerCooldown => _hammerCooldown;

        [SerializeField]
        private int _hammerSuccesfulHits = 5;

        public int HammerSuccessfulHits => _hammerSuccesfulHits;

        [SerializeField]
        private int _wrenchSuccesfulTurns = 5;

        public int WrenchSuccessfulTurns => _wrenchSuccesfulTurns;

        [SerializeField]
        private int[] _chargeTimePerPlayerCount;

        public IReadOnlyCollection<int> ChargeTimePerPlayerCount => _chargeTimePerPlayerCount;
#endregion

        [Space(10)]

#region Spawn Tags
        [Header("Spawn Tags")]

        [SerializeField]
        private string _repairableRobotSpawnTag = "repairable_robot";

        public string RepairableRobotSpawnTag => _repairableRobotSpawnTag;
#endregion

        [Space(10)]

#region Game Stuff
        [SerializeField]
        private int _repairTime = 30;

        public int RepairTime => _repairTime;

        [SerializeField]
        [Range(1.0f, 5.0f)]
        private float _robotRespawnRate = 5.0f;

        public float RobotRespawnRate => _robotRespawnRate;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _passingRepairPercent = 0.5f;

        public float PassingRepairPercent => _passingRepairPercent;

        [SerializeField]
        private int _maxFailures = 3;

        public int MaxFailures => _maxFailures;
#endregion

        [Space(10)]

#region Debug
        [SerializeField]
        private bool _enableTutorialInEditor;

        public bool EnableTutorialInEditor => _enableTutorialInEditor;
#endregion
    }
}
