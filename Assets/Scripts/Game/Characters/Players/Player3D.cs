using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.Game.Players.Input;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players
{
    [RequireComponent(typeof(Game.Players.NetworkPlayer))]
    public abstract class Player3D : Actor3D, IPlayer
    {
        public GameObject GameObject => gameObject;

#region Network
        public override bool IsLocalActor => NetworkPlayer.isLocalPlayer;

        // need this to hand off to the NetworkManager before instantiating
        [SerializeField]
        private Game.Players.NetworkPlayer _networkPlayer;

        public Game.Players.NetworkPlayer NetworkPlayer => _networkPlayer;
#endregion

#region Input / Behavior
        [SerializeField]
        private PlayerInputHandler _inputHandler;

        public PlayerInputHandler PlayerInputHandler => _inputHandler;

        [CanBeNull]
        public PlayerBehavior PlayerBehavior => (PlayerBehavior)Behavior;
#endregion

#region Viewer
        [CanBeNull]
        public IPlayerViewer PlayerViewer { get; protected set; }

        [CanBeNull]
        public Viewer Viewer
        {
            get => null == PlayerViewer ? null : PlayerViewer.Viewer;
            set {}
        }
#endregion

#region Unity Lifecycle
        protected override void OnDestroy()
        {
            if(null != Viewer && ViewerManager.HasInstance) {
                ViewerManager.Instance.ReleaseViewer(Viewer);
            }
            PlayerViewer = null;

            base.OnDestroy();
        }
#endregion

        public override void Initialize(Guid id)
        {
            base.Initialize(id);

            Assert.IsTrue(Behavior is PlayerBehavior);

            InitializeLocalPlayer(id);
        }

        protected virtual bool InitializeLocalPlayer(Guid id)
        {
            if(!IsLocalActor) {
                return false;
            }

            Debug.Log($"Initializing local player {id}");

            _inputHandler.Initialize(NetworkPlayer.ControllerId);

            NetworkPlayer.Init3D();

            PlayerBehavior.InitializeLocalPlayerBehavior();

            return true;
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            Debug.Log($"Spawning player (controller={NetworkPlayer.playerControllerId}, isLocalPlayer={IsLocalActor})");

            // spawnpoint doesn't initialize players, so we have to do it before calling the base OnSpawn
            Initialize(Guid.NewGuid());
            Behavior.Initialize(GameStateManager.Instance.PlayerManager.PlayerBehaviorData);

            if(!NetworkManager.Instance.IsClientActive()) {
                NetworkPlayer.RpcSpawn(Id.ToString());
            }

            return base.OnSpawn(spawnpoint);
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            Debug.Log($"Respawning player (controller={NetworkPlayer.playerControllerId}, isLocalPlayer={IsLocalActor})");

            return true;
        }
#endregion
    }
}
