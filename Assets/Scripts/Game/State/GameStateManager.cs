#pragma warning disable 0618    // disable obsolete warning for now

using System;
using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Loading;
using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.Game.NPCs;
using pdxpartyparrot.Game.Players;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.Assertions;

#if USE_NETWORKING
using UnityEngine.Networking;
#endif

namespace pdxpartyparrot.Game.State
{
    public sealed class GameStateManager : SingletonBehavior<GameStateManager>
    {
#region Game State Prefabs
        [Header("Game States")]

        [SerializeField]
        private MainMenuState _mainMenuStatePrefab;

        [SerializeField]
        private NetworkConnectState _networkConnectStatePrefab;

        [SerializeField]
        private SceneTester _sceneTesterStatePrefab;

        [SerializeField]
        [ReadOnly]
        private GameState _currentGameState;

        [CanBeNull]
        public GameState CurrentState => _currentGameState;

        [SerializeField]
        [ReadOnly]
        private SubGameState _currentSubGameState;

        [CanBeNull]
        public SubGameState CurrentSubState => _currentSubGameState;

        private readonly Stack<SubGameState> _subStateStack = new Stack<SubGameState>();
#endregion

#if ENABLE_SERVER_SPECTATOR
        [Space(10)]

#region Server Spectator
        [Header("Server Spectator")]

        [SerializeField]
        private ServerSpectator _serverSpectatorPrefab;

        public  ServerSpectator ServerSpectatorPrefab => _serverSpectatorPrefab;

        [SerializeField]
        private ServerSpectatorViewer _serverSpectatorViewerPrefab;

        public ServerSpectatorViewer ServerSpectatorViewerPrefab => _serverSpectatorViewerPrefab;
#endregion
#endif

#region Network
        [CanBeNull]
        public NetworkClient NetworkClient { get; set; }
#endregion

        [Space(10)]

#region Managers
        [Header("Registered Managers")]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private IGameManager _gameManager;

        [CanBeNull]
        public IGameManager GameManager => _gameManager;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private IGameUIManager _gameUIManager;

        [CanBeNull]
        public IGameUIManager GameUIManager => _gameUIManager;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private IPlayerManager _playerManager;

        [CanBeNull]
        public IPlayerManager PlayerManager => _playerManager;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private INPCManager _npcManager;

        [CanBeNull]
        public INPCManager NPCManager => _npcManager;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
// TODO: allocate and disable *all* game states

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            // OnDestroy() can't be a coroutine and this doesn't really need to happen
            // really we should just straight up destroy everything
            /*IEnumerator runner = ExitCurrentStateRoutine();
            while(runner.MoveNext()) {
                yield return null;
            }*/

// TODO: destroy all game states

            base.OnDestroy();
        }

        private void Update()
        {
            float dt = Time.deltaTime;

            if(null != _currentSubGameState) {
                _currentSubGameState.OnUpdate(dt);
            } else if(null != _currentGameState) {
                _currentGameState.OnUpdate(dt);
            }
        }
#endregion

#region Register Game Managers
        public void RegisterGameManager(IGameManager gameManager)
        {
            Assert.IsNull(_gameManager);
            _gameManager = gameManager;
        }

        public void UnregisterGameManager()
        {
            _gameManager = null;
        }

        public void RegisterGameUIManager(IGameUIManager gameUIManager)
        {
            Assert.IsNull(_gameUIManager);
            _gameUIManager = gameUIManager;
        }

        public void UnregisterGameUIManager()
        {
            _gameUIManager = null;
        }

        public void RegisterPlayerManager(IPlayerManager playerManager)
        {
            Assert.IsNull(_playerManager);
            _playerManager = playerManager;
        }

        public void UnregisterPlayerManager()
        {
            _playerManager = null;
        }

        public void RegisterNPCManager(INPCManager npcManager)
        {
            Assert.IsNull(_npcManager);
            _npcManager = npcManager;
        }

        public void UnregisterNPCManager()
        {
            _npcManager = null;
        }
#endregion

        public void ShutdownNetwork()
        {
            if(Core.Network.NetworkManager.HasInstance) {
                Core.Network.NetworkManager.Instance.Stop();
            }

            NetworkClient = null;
        }

#region State Management
        public void TransitionToInitialStateAsync(Action<GameState> initializeState=null, Action onStateLoaded=null)
        {
            Debug.Log("Transition to initial state (main menu)...");
            TransitionStateAsync(_mainMenuStatePrefab, initializeState, onStateLoaded);
        }

        public void TransitionStateAsync<TV>(TV gameStatePrefab, Action<TV> initializeState=null, Action onStateLoaded=null) where TV: GameState
        {
            StartCoroutine(TransitionStateRoutine(gameStatePrefab, initializeState, onStateLoaded));
        }

        private IEnumerator TransitionStateRoutine<TV>(TV gameStatePrefab, Action<TV> initializeState=null, Action onStateLoaded=null) where TV: GameState
        {
            PartyParrotManager.Instance.LoadingManager.ShowLoadingScreen(true);

            IEnumerator exitRunner = ExitCurrentStateRoutine();
            while(exitRunner.MoveNext()) {
                yield return null;
            }

            // TODO: this should enable the state from the set rather than allocating
            TV gameState = Instantiate(gameStatePrefab, transform);
            initializeState?.Invoke(gameState);

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.0f, $"Loading game state scene '{gameState.CurrentSceneName}'...");
            yield return null;

            IEnumerator<float> runner = gameState.LoadSceneRoutine();
            while(runner.MoveNext()) {
                PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(runner.Current * 0.5f, $"Loading game state scene '{gameState.CurrentSceneName}'...");
                yield return null;
            }

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.5f, $"Game state scene '{gameState.CurrentSceneName}' loaded!");
            yield return null;

            _currentGameState = gameState;
            IEnumerator<LoadStatus> enterRunner = _currentGameState.OnEnterRoutine();
            while(enterRunner.MoveNext()) {
                LoadStatus status = enterRunner.Current;
                if(null != status) {
                    PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.5f + status.LoadPercent * 0.5f, status.Status);
                }
                yield return null;
            }

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(1.0f, "State transition complete!");
            yield return null;

            PartyParrotManager.Instance.LoadingManager.ShowLoadingScreen(false);

            _currentGameState.OnEnter();

            onStateLoaded?.Invoke();
        }

        private IEnumerator ExitCurrentStateRoutine()
        {
            while(null != _currentSubGameState || _subStateStack.Count > 0) {
                PopSubState();
            }

            if(null == _currentGameState) {
                yield break;
            }

            _currentGameState.OnExit();

            GameState gameState = _currentGameState;
            _currentGameState = null;

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.0f, "Unloading current scene...");
            yield return null;

            IEnumerator<float> runner = gameState.UnloadSceneRoutine();
            while(runner.MoveNext()) {
                PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(runner.Current * 0.5f, "Unloading current scene...");
                yield return null;
            }

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.5f, "Unloading current scene...");
            yield return null;

            IEnumerator<LoadStatus> exitRunner = gameState.OnExitRoutine();
            while(exitRunner.MoveNext()) {
                LoadStatus status = exitRunner.Current;
                if(null != status) {
                    PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(0.5f + status.LoadPercent * 0.5f, status.Status);
                }
                yield return null;
            }

            PartyParrotManager.Instance.LoadingManager.UpdateLoadingScreen(1.0f, "Current scene unloaded!");
            yield return null;

            // TODO: disable the state, don't destroy it
            Destroy(gameState.gameObject);
        }

        public void PushSubState<TV>(TV gameStatePrefab, Action<TV> initializeState=null) where TV: SubGameState
        {
            if(null != _currentSubGameState) {
                _currentSubGameState.OnPause();
            } else if(null != _currentGameState) {
                _currentGameState.OnPause();
            }

            // enqueue the current state if we have one
            if(null != _currentSubGameState) {
                _subStateStack.Push(_currentSubGameState);
            }

            // new state is now the current state
            // TODO: this should enable the state from the set rather than allocating
            TV gameState = Instantiate(gameStatePrefab, transform);
            initializeState?.Invoke(gameState);

            _currentSubGameState = gameState;
            _currentSubGameState.OnEnter();
        }

        public void PopSubState()
        {
            SubGameState previousState = _currentSubGameState;
            _currentSubGameState = null;

            if(null != previousState) {
                previousState.OnExit();

                // TODO: this should disable the state rather than destroying it
                Destroy(previousState.gameObject);
            }

            _currentSubGameState = _subStateStack.Count > 0 ? _subStateStack.Pop() : null;

            if(null != _currentSubGameState) {
                _currentSubGameState.OnResume();
            } else if(null != _currentGameState) {
                _currentGameState.OnResume();
            }
        }
#endregion

#region Start Game
        public void StartLocal(MainGameState mainGameStatePrefab, Action<GameState> gameStateInit=null)
        {
            PushSubState(_networkConnectStatePrefab, state => {
                state.Initialize(NetworkConnectState.ConnectType.Local, mainGameStatePrefab, gameStateInit);
            });
        }

#if USE_NETWORKING
        public void StartHost(MainGameState mainGameStatePrefab, Action<GameState> gameStateInit=null)
        {
            PushSubState(_networkConnectStatePrefab, state => {
                state.Initialize(NetworkConnectState.ConnectType.Server, mainGameStatePrefab, gameStateInit);
            });
        }

        public void StartJoin(MainGameState mainGameStatePrefab, Action<GameState> gameStateInit=null)
        {
            PushSubState(_networkConnectStatePrefab, state => {
                state.Initialize(NetworkConnectState.ConnectType.Client, mainGameStatePrefab, gameStateInit);
            });
        }
#endif
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Game.GameStateManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.Label($"Current Game State: {(null == CurrentState ? "" : CurrentState.Name)}");
                GUILayout.Label($"Current Sub Game State: {(null == CurrentSubState ? "" : CurrentSubState.Name)}");

                if(GUIUtils.LayoutButton("Reset")) {
                    TransitionToInitialStateAsync();
                    return;
                }

#if USE_NETWORKING
                if(null != NetworkClient) {
                    GUILayout.BeginVertical("Client Stats", GUI.skin.box);
                        GUILayout.Label($"Ping: {NetworkClient.GetRTT()}ms");

                        NetworkClient.GetStatsIn(out int numMsgs, out int numBytes);
                        GUILayout.Label($"Messages received: {numMsgs}");
                        GUILayout.Label($"Bytes received: {numBytes}");

                        NetworkClient.GetStatsOut(out numMsgs, out int numBufferedMsgs, out numBytes, out int lastBufferedPerSecond);
                        GUILayout.Label($"Messages sent: {numMsgs}");
                        GUILayout.Label($"Messages buffered: {numBufferedMsgs}");
                        GUILayout.Label($"Bytes sent: {numBytes}");
                        GUILayout.Label($"Messages buffered per second: {lastBufferedPerSecond}");
                    GUILayout.EndVertical();
                }

                if(Core.Network.NetworkManager.Instance.IsServerActive()) {
                    GUILayout.BeginVertical("Server Stats", GUI.skin.box);
                        NetworkServer.GetStatsIn(out int numMsgs, out int numBytes);
                        GUILayout.Label($"Messages received: {numMsgs}");
                        GUILayout.Label($"Bytes received: {numBytes}");

                        NetworkServer.GetStatsOut(out numMsgs, out int numBufferedMsgs, out numBytes, out int lastBufferedPerSecond);
                        GUILayout.Label($"Messages sent: {numMsgs}");
                        GUILayout.Label($"Messages buffered: {numBufferedMsgs}");
                        GUILayout.Label($"Bytes sent: {numBytes}");
                        GUILayout.Label($"Messages buffered per second: {lastBufferedPerSecond}");
                    GUILayout.EndVertical();
                }
#endif
            };

            DebugMenuNode testSceneDebugMenuNode = DebugMenuManager.Instance.AddNode(() => "Game.GameStateManager.TestScenes");
            testSceneDebugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical("Test Scenes", GUI.skin.box);
                    foreach(string sceneName in _sceneTesterStatePrefab.TestScenes) {
                        if(GUIUtils.LayoutButton($"Load Test Scene '{sceneName}'")) {
                            Debug.Log($"Loading test scene '{sceneName}'...");
                            TransitionToInitialStateAsync(null, () => {
                                PushSubState(_networkConnectStatePrefab, connectState => {
                                        connectState.Initialize(NetworkConnectState.ConnectType.Local, _sceneTesterStatePrefab, state => {
                                            SceneTester sceneTester = (SceneTester)state;
                                            sceneTester.SetScene(sceneName);
                                        });
                                    });
                            });
                            break;
                        }
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
