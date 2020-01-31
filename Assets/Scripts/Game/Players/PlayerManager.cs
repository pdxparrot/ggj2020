#pragma warning disable 0618    // disable obsolete warning for now

using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Data.Characters;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Players
{
    public interface IPlayerManager
    {
        bool PlayersImmune { get; }

        bool DebugInput { get; }

        CharacterBehaviorData PlayerBehaviorData { get; }

        IReadOnlyCollection<IPlayer> Players { get; }

        void DespawnPlayers();
    }

    public abstract class PlayerManager<T> : SingletonBehavior<T>, IPlayerManager where T: PlayerManager<T>
    {
#region Debug
        [SerializeField]
        private bool _playersImmune;

        public bool PlayersImmune => _playersImmune;

        [SerializeField]
        private bool _debugInput;

        public bool DebugInput => _debugInput;
#endregion

        // TODO: what if we have different types of players?
        [SerializeField]
        private CharacterBehaviorData _playerBehaviorData;

        public CharacterBehaviorData PlayerBehaviorData => _playerBehaviorData;

        [SerializeField]
        private Actor _playerActorPrefab;

        private Actor PlayerActorPrefab => _playerActorPrefab;

        private IPlayer PlayerPrefab => (IPlayer)_playerActorPrefab;

        private readonly HashSet<IPlayer> _players = new HashSet<IPlayer>();

        public IReadOnlyCollection<IPlayer> Players => _players;

        private GameObject _playerContainer;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            Assert.IsTrue(_playerBehaviorData is PlayerBehaviorData);

            _playerContainer = new GameObject("Players");

            Core.Network.NetworkManager.Instance.RegisterPlayerPrefab(PlayerPrefab.NetworkPlayer);
#if USE_NETWORKING
            Core.Network.NetworkManager.Instance.ServerAddPlayerEvent += ServerAddPlayerEventHandler;
#endif

            InitDebugMenu();

            GameStateManager.Instance.RegisterPlayerManager(this);
        }

        protected override void OnDestroy()
        {
            if(GameStateManager.HasInstance) {
                GameStateManager.Instance.UnregisterPlayerManager();
            }

            DestroyDebugMenu();

            if(Core.Network.NetworkManager.HasInstance) {
#if USE_NETWORKING
                Core.Network.NetworkManager.Instance.ServerAddPlayerEvent -= ServerAddPlayerEventHandler;
#endif
                Core.Network.NetworkManager.Instance.UnregisterPlayerPrefab();
            }

            Destroy(_playerContainer);
            _playerContainer = null;

            base.OnDestroy();
        }
#endregion


        private void SpawnPlayer(NetworkConnection conn, short controllerId)
        {
            Assert.IsTrue(NetworkManager.Instance.IsServerActive());

            Debug.Log($"Spawning player for controller {controllerId}...");

            SpawnPoint spawnPoint = SpawnManager.Instance.GetPlayerSpawnPoint(controllerId);
            if(null == spawnPoint) {
                Debug.LogError("Failed to get player spawnpoint!");
                return;
            }

            NetworkPlayer player = Core.Network.NetworkManager.Instance.SpawnPlayer<NetworkPlayer>(controllerId, conn, _playerContainer.transform);
            if(null == player) {
                Debug.LogError("Failed to spawn network player!");
                return;
            }
            player.Initialize(controllerId);

            if(!spawnPoint.SpawnPlayer((Actor)player.Player)) {
                Debug.LogError("Failed to spawn player!");
                return;
            }

            _players.Add(player.Player);
        }

        public bool RespawnPlayer(IPlayer player)
        {
            Assert.IsTrue(NetworkManager.Instance.IsServerActive());

            Debug.Log($"Respawning player {player.Id}");

            SpawnPoint spawnPoint = SpawnManager.Instance.GetPlayerSpawnPoint(player.NetworkPlayer.playerControllerId);
            if(null == spawnPoint) {
                Debug.LogError("Failed to get player spawnpoint!");
                return false;
            }

            return spawnPoint.ReSpawn((Actor)player);
        }

        // TODO: figure out how to work this in when players disconnect
        public void DespawnPlayer(IPlayer player)
        {
            Assert.IsTrue(NetworkManager.Instance.IsServerActive());

            Debug.Log($"Despawning player {player.Id}");

            _players.Remove(player);
        }

        // TODO: figure out how to even do this
        public void DespawnPlayers()
        {
            if(Players.Count < 1) {
                return;
            }

            Assert.IsTrue(NetworkManager.Instance.IsServerActive());

            Debug.Log($"Despawning {Players.Count} players...");

            // TODO: how?

            _players.Clear();
        }

#region Event Handlers
#if USE_NETWORKING
        private void ServerAddPlayerEventHandler(object sender, ServerAddPlayerEventArgs args)
        {
            SpawnPlayer(args.NetworkConnection, args.PlayerControllerId);
        }
#endif
#endregion

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Game.PlayerManager");
            _debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Players", GUI.skin.box);
                    foreach(IPlayer player in _players) {
                        GUILayout.Label($"{player.Id} {player.Movement.Position}");
                    }
                GUILayout.EndVertical();

                _playersImmune = GUILayout.Toggle(_playersImmune, "Players Immune");
                _debugInput = GUILayout.Toggle(_debugInput, "Debug Input");
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
