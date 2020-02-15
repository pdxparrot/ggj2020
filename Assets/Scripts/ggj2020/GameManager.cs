using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game;
using pdxpartyparrot.ggj2020.Camera;
using pdxpartyparrot.ggj2020.Data;
using pdxpartyparrot.ggj2020.Level;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020
{
    public sealed class GameManager : GameManager<GameManager>
    {
#region Events
        public event EventHandler<EventArgs> MechanicsCanInteractEvent;

        public event EventHandler<EventArgs> RepairSuccessEvent;
        public event EventHandler<EventArgs> RepairFailureEvent;

        public event EventHandler<EventArgs> RobotImpulseEvent;
#endregion

        public GameData GameGameData => (GameData)GameData;

        // TODO: this should be a base class
        [CanBeNull]
        public RepairBayLevel GameLevelHelper => (RepairBayLevel)LevelHelper;

        // only valid on the client
        public GameViewer Viewer { get; private set; }

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _mechanicsCanInteract;

        public bool MechanicsCanInteract
        {
            get => _mechanicsCanInteract;
            set
            {
                _mechanicsCanInteract = value;
                MechanicsCanInteractEvent?.Invoke(this, EventArgs.Empty);
            }
        }

        [SerializeField]
        [ReadOnly]
        private int _repairSuccesses;

        public int RepairSuccesses => _repairSuccesses;

        [SerializeField]
        [ReadOnly]
        private int _repairFailures;

        public int RepairFailures => _repairFailures;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(GameGameData.RepairableRobotData.InitialDamagedAreasPerPlayerCount.Count == GameGameData.MaxLocalPlayers);
            Assert.IsTrue(GameGameData.RepairableRobotData.DamageAreaIncreasePerPlayerCount.Count == GameGameData.MaxLocalPlayers);

            base.Awake();

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }
#endregion

        public override void TransitionScene(string nextScene, Action onComplete)
        {
            base.TransitionScene(nextScene, () => {
                // TODO: this is gross and seems wrong
                StartGameServer();
                StartGameClient();

                onComplete?.Invoke();
            });
        }

        public override void GameReady()
        {
            base.GameReady();

            _repairSuccesses = 0;
            _repairFailures = 0;
        }

        //[Client]
        public void InitViewer()
        {
            Viewer = ViewerManager.Instance.AcquireViewer<GameViewer>();
            if(null == Viewer) {
                Debug.LogWarning("Unable to acquire game viewer!");
                return;
            }
            Viewer.Initialize(GameGameData);
        }

        public void RobotImpulse()
        {
            RobotImpulseEvent?.Invoke(this, EventArgs.Empty);
        }

        public void RepairSuccess(float repairPercent)
        {
            Debug.Log($"Repair success {repairPercent} of {GameGameData.PassingRepairPercent}");

            _repairSuccesses++;

            RepairSuccessEvent?.Invoke(this, EventArgs.Empty);
        }

        public bool RepairFailure(float repairPercent)
        {
            Debug.Log($"Repair failure {repairPercent} of {GameGameData.PassingRepairPercent}");

            _repairFailures++;

            RepairFailureEvent?.Invoke(this, EventArgs.Empty);

            if(_repairFailures >= GameGameData.MaxFailures) {
                GameOver();
                return false;
            }

            return true;
        }

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ggj2020.GameManager");
            _debugMenuNode.RenderContentsAction = () => {
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
