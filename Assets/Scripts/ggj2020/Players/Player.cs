using System;

using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ggj2020.Camera;
using pdxpartyparrot.ggj2020.UI;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class Player : Player25D
    {
        public PlayerInputHandler GamePlayerInput => (PlayerInputHandler)PlayerInputHandler;

        public PlayerBehavior GamePlayerBehavior => (PlayerBehavior)PlayerBehavior;

        private GameViewer PlayerGameViewer => (GameViewer)Viewer;

        [Space(10)]

        [SerializeField]
        private MechanicModel _mechanicModel;

        public MechanicModel MechanicModel => _mechanicModel;

        [SerializeField]
        private UIBubble _uiBubble;

        public UIBubble UIBubble => _uiBubble;

        private SpawnPoint _spawnpoint;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInputHandler is PlayerInputHandler);

            _uiBubble.HideSprite();
        }
#endregion

        public override void Initialize(Guid id)
        {
            base.Initialize(id);

            Assert.IsTrue(PlayerBehavior is PlayerBehavior);
        }

        protected override bool InitializeLocalPlayer(Guid id)
        {
            if(!base.InitializeLocalPlayer(id)) {
                return false;
            }

            PlayerViewer = GameManager.Instance.Viewer;

            _mechanicModel.Initialize(NetworkPlayer.ControllerId);

            return true;
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            Assert.IsNull(_spawnpoint);

            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            if(!spawnpoint.Acquire(this)) {
                return false;
            }
            _spawnpoint = spawnpoint;

            PlayerGameViewer.AddTarget(this);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            Assert.IsNull(_spawnpoint);

            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            if(!spawnpoint.Acquire(this)) {
                return false;
            }
            _spawnpoint = spawnpoint;

            PlayerGameViewer.AddTarget(this);

            return true;
        }

        public override void OnDeSpawn()
        {
            _spawnpoint.Release();
            _spawnpoint = null;

            PlayerGameViewer.RemoveTarget(this);

            base.OnDeSpawn();
        }
#endregion
    }
}
