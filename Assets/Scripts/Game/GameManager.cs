using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.Level;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game
{
    public interface IGameManager
    {
#region Events
        event EventHandler<EventArgs> GameStartServerEvent;
        event EventHandler<EventArgs> GameStartClientEvent;

        event EventHandler<EventArgs> GameReadyEvent;
        event EventHandler<EventArgs> GameUnReadyEvent;
        event EventHandler<EventArgs> GameOverEvent;
#endregion

        GameData GameData { get; }

        CreditsData CreditsData { get; }

        bool IsGameReady { get; }

        bool IsGameOver { get; }

        bool TransitionToHighScores { get; }

        void Initialize();

        void Shutdown();

        void RegisterLevelHelper(LevelHelper levelHelper);

        void UnRegisterLevelHelper(LevelHelper levelHelper);

        void GameReady();

        void GameUnReady();

        void GameOver();

        void TransitionScene(string nextScene, Action onComplete);
    }

    public abstract class GameManager<T> : SingletonBehavior<T>, IGameManager where T: GameManager<T>
    {
#region Events
        public event EventHandler<EventArgs> GameStartServerEvent;
        public event EventHandler<EventArgs> GameStartClientEvent;

        public event EventHandler<EventArgs> GameReadyEvent;
        public event EventHandler<EventArgs> GameUnReadyEvent;
        public event EventHandler<EventArgs> GameOverEvent;
#endregion

        [SerializeField]
        private GameData _gameData;

        public GameData GameData => _gameData;

        [SerializeField]
        private CreditsData _creditsData;

        public CreditsData CreditsData => _creditsData;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private LevelHelper _levelHelper;

        [CanBeNull]
        public LevelHelper LevelHelper => _levelHelper;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _isGameReady;

        public virtual bool IsGameReady
        {
            get => _isGameReady;
            private set => _isGameReady = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _isGameOver;

        public virtual bool IsGameOver
        {
            get => _isGameOver;
            private set => _isGameOver = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _transitionToHighScores;

        public bool TransitionToHighScores
        {
            get => _transitionToHighScores;
            set => _transitionToHighScores = value;
        }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            GameStateManager.Instance.RegisterGameManager(this);
        }

        protected override void OnDestroy()
        {
            if(GameStateManager.HasInstance) {
                GameStateManager.Instance.UnregisterGameManager();
            }

            base.OnDestroy();
        }
#endregion

        public virtual void Initialize()
        {
            IsGameOver = false;
            IsGameReady = false;
            TransitionToHighScores = false;

            InitializeObjectPools();
        }

        public virtual void Shutdown()
        {
            IsGameOver = false;
            IsGameReady = false;

            DestroyObjectPools();

            if(Core.Network.NetworkManager.Instance.IsServerActive() && null != GameStateManager.Instance.PlayerManager) {
                GameStateManager.Instance.PlayerManager.DespawnPlayers();
            }
        }

#region Level Helper
        public void RegisterLevelHelper(LevelHelper levelHelper)
        {
            Assert.IsNull(_levelHelper);
            _levelHelper = levelHelper;
        }

        public void UnRegisterLevelHelper(LevelHelper levelHelper)
        {
            Assert.IsTrue(levelHelper == _levelHelper);
            _levelHelper = null;
        }
#endregion

        protected virtual void InitializeObjectPools()
        {
            if(null != GameData.FloatingTextPrefab) {
                PooledObject pooledObject = GameData.FloatingTextPrefab.GetComponent<PooledObject>();
                ObjectPoolManager.Instance.InitializePoolAsync(GameStateManager.Instance.GameUIManager.DefaultFloatingTextPoolName, pooledObject, GameData.FloatingTextPoolSize);
            }
        }

        protected virtual void DestroyObjectPools()
        {
            if(ObjectPoolManager.HasInstance) {
                ObjectPoolManager.Instance.DestroyPool(GameStateManager.Instance.GameUIManager.DefaultFloatingTextPoolName);
            }
        }

        public virtual void StartGameServer()
        {
            Debug.Log("Start Game (Server)");

            GameStartServerEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void StartGameClient()
        {
            Debug.Log("Start Game (Client)");

            GameStartClientEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameReady()
        {
            Debug.Log("Game Ready");

            IsGameReady = true;

            GameReadyEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameUnReady()
        {
            Debug.Log("Game UnReady");

            IsGameReady = false;

            GameUnReadyEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameOver()
        {
            Debug.Log("Game Over");

            IsGameOver = true;

            GameOverEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void TransitionScene(string nextScene, Action onComplete)
        {
            GameStateManager.Instance.CurrentState.ChangeSceneAsync(nextScene, onComplete);
        }
    }
}
