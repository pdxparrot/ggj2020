using System;

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

        [SerializeField]
        private int _repairTime = 30;

        public int RepairTime => _repairTime;

        [Space(10)]

        [SerializeField]
        private RepairableRobot _repairableRobotPrefab;

        public RepairableRobot RepairableRobotPrefab => _repairableRobotPrefab;

        [SerializeField]
        private RepairableRobotData _repairableRobotData;

        public RepairableRobotData RepairableRobotData => _repairableRobotData;

        [SerializeField]
        private string _repairableRobotSpawnTag = "repairable_robot";

        public string RepairableRobotSpawnTag => _repairableRobotSpawnTag;

        [SerializeField]
        private string _playerFighterSpawnTag = "player_fighter";

        public string PlayerFighterSpawnTag => _playerFighterSpawnTag;

        [SerializeField]
        private string _npcFighterSpawnTag = "npc_fighter";

        public string NPCFighterSpawnTag => _npcFighterSpawnTag;
    }
}
