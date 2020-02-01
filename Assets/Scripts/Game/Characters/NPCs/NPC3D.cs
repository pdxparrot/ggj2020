using System;
using System.Collections;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class NPC3D : Actor3D, INPC
    {
        public GameObject GameObject => gameObject;

#region Network
        public override bool IsLocalActor => false;
#endregion

#region Behavior
        [CanBeNull]
        public NPCBehavior NPCBehavior => (NPCBehavior)Behavior;
#endregion

        [Space(10)]

#region Pathing
        [SerializeField]
        [ReadOnly]
        private bool _pathPending;

        [SerializeField]
        [ReadOnly]
        private bool _hasPath;

        [SerializeField]
        [ReadOnly]
        private NavMeshPathStatus _pathStatus;

        [SerializeField]
        [ReadOnly]
        private bool _isPathStale;

        public bool HasPath => _agent.hasPath;

        public Vector3 NextPosition => _agent.nextPosition;
#endregion

        [CanBeNull]
        private PooledObject _pooledObject;

        private NavMeshAgent _agent;

        private Coroutine _agentStuckCheck;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastStuckCheckPosition;

        [SerializeField]
        [ReadOnly]
        private int _stuckCheckCount;

#region Debug
        [SerializeField]
        [CanBeNull]
        private Transform _debugTextTarget;

        [SerializeField]
        [CanBeNull]
        private FloatingTextQueue _debugTextQueue;

#if UNITY_EDITOR
        private LineRenderer _debugPathRenderer;
#endif
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _agent = GetComponent<NavMeshAgent>();

            _pooledObject = GetComponent<PooledObject>();
            if(null != _pooledObject) {
                _pooledObject.RecycleEvent += RecycleEventHandler;
            }

#if UNITY_EDITOR
            _debugPathRenderer = gameObject.AddComponent<LineRenderer>();
            _debugPathRenderer.shadowCastingMode = ShadowCastingMode.Off;
            _debugPathRenderer.receiveShadows = false;
            _debugPathRenderer.allowOcclusionWhenDynamic = false;
            _debugPathRenderer.startWidth = 0.1f;
            _debugPathRenderer.endWidth = 0.1f;
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            DebugRenderPath();
#endif

            _pathStatus = _agent.pathStatus;
            _pathPending = _agent.pathPending;
            _hasPath = _agent.hasPath;
            _isPathStale = _agent.isPathStale;
        }

        private void LateUpdate()
        {
            // TODO: this works great except that
            // the player can abuse the NPC re-accelerating
            // by pausing and unpausing the game
            if(null != NPCBehavior && !NPCBehavior.CanMove) {
                Stop(false, false);
            }
        }
#endregion

        public override void Initialize(Guid id)
        {
            base.Initialize(id);

            Assert.IsTrue(Behavior is NPCBehavior);

            // TODO: this might crash now that the behavior is not initialized by this
            _agent.speed = NPCBehavior.NPCBehaviorData.MoveSpeed;
            _agent.angularSpeed = NPCBehavior.NPCBehaviorData.AngularMoveSpeed;
            _agent.acceleration = NPCBehavior.NPCBehaviorData.MoveAcceleration;
            _agent.stoppingDistance = Radius + NPCBehavior.NPCBehaviorData.StoppingDistance;
            _agent.autoBraking = true;

            _agent.radius = Radius;
            _agent.height = Height;

            _agentStuckCheck = StartCoroutine(AgentStuckCheck());
        }

#region Pathing
        public bool UpdatePath(Vector3 target)
        {
            if(!_agent.SetDestination(target)) {
                Debug.LogWarning($"Failed to set NPC {Id} destination: {target}");
                return false;
            }

            if(GameStateManager.Instance.NPCManager.DebugBehavior) {
                DisplayDebugText($"Pathing to {target}", Color.green);
            }

            return true;
        }

        public void ResetPath(bool idle)
        {
            _agent.ResetPath();

            if(idle) {
                NPCBehavior.OnIdle();
            }
        }

#if UNITY_EDITOR
        private void DebugRenderPath()
        {
            if(!_agent.hasPath) {
                _debugPathRenderer.positionCount = 0;
                return;
            }

            _debugPathRenderer.positionCount = _agent.path.corners.Length;
            for(int i=0; i<_agent.path.corners.Length; ++i) {
                _debugPathRenderer.SetPosition(i, _agent.path.corners[i]);
            }
        }
#endif
#endregion

        public void Stop(bool resetPath, bool idle)
        {
            _agent.velocity = Vector3.zero;

            if(resetPath) {
                ResetPath(idle);
            } else if(idle) {
                NPCBehavior.OnIdle();
            }
        }

        public void Recycle()
        {
            NPCBehavior.OnRecycle();
            if(null != _pooledObject) {
                _pooledObject.Recycle();
            }
        }

        private IEnumerator AgentStuckCheck()
        {
            _lastStuckCheckPosition = Behavior.Owner.Movement.Position;
            _stuckCheckCount = 0;

            WaitForSeconds wait = new WaitForSeconds(GameStateManager.Instance.NPCManager.StuckCheckSeconds);
            while(true) {
                // if we don't have a path, we can't technically be stuck
                if(!_agent.hasPath || _agent.pathPending) {
                    _lastStuckCheckPosition = Behavior.Owner.Movement.Position;
                    _stuckCheckCount = 0;

                    yield return wait;
                    continue;
                }

                // see if we've moved at least our stopping distance
                // TODO: figure out how far we *should* have moved and go a little less than that
                Vector3 position = Behavior.Owner.Movement.Position;
                if((position - _lastStuckCheckPosition).sqrMagnitude < (_agent.stoppingDistance * _agent.stoppingDistance)) {
                    _stuckCheckCount += 1;
                } else {
                    _lastStuckCheckPosition = position;
                    _stuckCheckCount = 0;
                }

                // are we stuck?
                if(_stuckCheckCount >= GameStateManager.Instance.NPCManager.StuckCheckMaxPasses) {
                    if(GameStateManager.Instance.NPCManager.DebugBehavior) {
                        DisplayDebugText("Stuck", Color.magenta);
                    }

                    Stop(true, true);
                }

                yield return wait;
            }
        }

#region Event Handlers
        private void RecycleEventHandler(object sender, EventArgs args)
        {
            StopCoroutine(_agentStuckCheck);
            _agentStuckCheck = null;

            OnDeSpawn();
        }
#endregion

#region Debug
        public void DisplayDebugText(string text, Color color)
        {
            Debug.Log($"[NPC {Id}]: {text}");
            if(null != _debugTextQueue) {
                _debugTextQueue.QueueFloatingText(text, color, () => null == _debugTextTarget ? transform.position : _debugTextTarget.position);
            }
        }
#endregion
    }
}
