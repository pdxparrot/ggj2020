using System;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.State;

using UnityEngine;

#if USE_NAVMESH
using UnityEngine.AI;
#endif

namespace pdxpartyparrot.Game.Level
{
#if USE_NAVMESH
    [RequireComponent(typeof(NavMeshSurface))]
#endif
    public abstract class LevelHelper : MonoBehaviour
    {
        [SerializeField]
        private string _nextLevel;

        [Space(10)]

        [SerializeField]
        private EffectTrigger _levelEnterEffect;

        [SerializeField]
        private EffectTrigger _levelExitEffect;

#if USE_NAVMESH
        private NavMeshSurface _navMeshSurface;
#endif

#region Unity Lifecycle
        protected virtual void Awake()
        {
#if USE_NAVMESH
            _navMeshSurface = GetComponent<NavMeshSurface>();
#endif

            GameStateManager.Instance.GameManager.RegisterLevelHelper(this);

            GameStateManager.Instance.GameManager.GameStartServerEvent += GameStartServerEventHandler;
            GameStateManager.Instance.GameManager.GameStartClientEvent += GameStartClientEventHandler;
            GameStateManager.Instance.GameManager.GameReadyEvent += GameReadyEventHandler;
            GameStateManager.Instance.GameManager.GameOverEvent += GameOverEventHandler;
        }

        protected virtual void OnDestroy()
        {
            if(GameStateManager.HasInstance && null != GameStateManager.Instance.GameManager) {
                GameStateManager.Instance.GameManager.GameOverEvent -= GameOverEventHandler;
                GameStateManager.Instance.GameManager.GameReadyEvent -= GameReadyEventHandler;
                GameStateManager.Instance.GameManager.GameStartClientEvent -= GameStartClientEventHandler;
                GameStateManager.Instance.GameManager.GameStartServerEvent -= GameStartServerEventHandler;

                GameStateManager.Instance.GameManager.UnRegisterLevelHelper(this);
            }
        }
#endregion

        protected void TransitionLevel()
        {
            // load the next level if we have one
            if(!string.IsNullOrWhiteSpace(_nextLevel)) {
                if(null != _levelExitEffect) {
                    _levelExitEffect.Trigger(DoLevelTransition);
                } else {
                    DoLevelTransition();
                }
            } else {
                GameStateManager.Instance.GameManager.GameOver();
            }
        }

        private void DoLevelTransition()
        {
            GameStateManager.Instance.GameManager.GameUnReady();
            GameStateManager.Instance.GameManager.TransitionScene(_nextLevel, null);
        }

#region Event Handlers
        protected virtual void GameStartServerEventHandler(object sender, EventArgs args)
        {
            Debug.Log("[Level] Server start...");

            // TODO: better to do this before we drop the loading screen and spawn stuff
#if USE_NAVMESH
            Debug.Log("[Level] Building nav mesh...");
            _navMeshSurface.BuildNavMesh();
#endif

            SpawnManager.Instance.Initialize();
        }

        protected virtual void GameStartClientEventHandler(object sender, EventArgs args)
        {
            Debug.Log("[Level] Client start...");

            // TODO: we really should communicate our ready state to the server
            // and then have it communicate back to us when everybody is ready
            if(null != _levelEnterEffect) {
                _levelEnterEffect.Trigger(GameStateManager.Instance.GameManager.GameReady);
            } else {
                GameStateManager.Instance.GameManager.GameReady();
            }
        }

        protected virtual void GameReadyEventHandler(object sender, EventArgs args)
        {
        }

        protected virtual void GameOverEventHandler(object sender, EventArgs args)
        {
        }
#endregion
    }
}
