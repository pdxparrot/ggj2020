using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Animation;
using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;

using UnityEngine;

namespace pdxpartyparrot.Core.Actors.Components
{
    // TODO: rename ActorBehaviorComponent
    public abstract class ActorBehaviorComponent : ActorComponent
    {
        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        protected ActorBehaviorComponentData _behaviorData;

        [CanBeNull]
        public ActorBehaviorComponentData BehaviorData => _behaviorData;

        [Space(10)]

#region Animation
        [Header("Animation")]

#if USE_SPINE
        [SerializeField]
        [CanBeNull]
        private SpineAnimationHelper _spineAnimationHelper;

        [CanBeNull]
        public SpineAnimationHelper SpineAnimationHelper => _spineAnimationHelper;

        [SerializeField]
        [CanBeNull]
        private SpineSkinHelper _spineSkinHelper;

        [CanBeNull]
        public SpineSkinHelper SpineSkinHelper => _spineSkinHelper;
#endif

        [SerializeField]
        [CanBeNull]
        private SpriteAnimationHelper _spriteAnimationHelper;

        [CanBeNull]
        public SpriteAnimationHelper SpriteAnimationHelper => _spriteAnimationHelper;

        [SerializeField]
        [CanBeNull]
        private Animator _animator;

        [CanBeNull]
        public Animator Animator => _animator;

        [SerializeField]
        private bool _pauseAnimationOnPause = true;

        protected bool PauseAnimationOnPause => _pauseAnimationOnPause;
#endregion

        [Space(10)]

#region Effects
        [Header("Actor Effects")]

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _spawnEffect;

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _respawnEffect;

        [SerializeField]
        [CanBeNull]
        protected EffectTrigger _despawnEffect;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _isAlive;

        public bool IsAlive => _isAlive;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;
        }

        protected override void OnDestroy()
        {
            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }

            base.OnDestroy();
        }

        protected virtual void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            AnimationUpdate(dt);
        }

        protected virtual void FixedUpdate()
        {
            float dt = UnityEngine.Time.fixedDeltaTime;

            PhysicsUpdate(dt);

            // fixes sketchy rigidbody angular momentum shit
            if(null != Owner && null != Owner.Movement) {
                Owner.Movement.ResetAngularVelocity();
            }
        }
#endregion

        public virtual void Initialize(ActorBehaviorComponentData behaviorData)
        {
            _behaviorData = behaviorData;

            if(null != Owner) {
                Owner.IsMoving = false;

                if(null != Owner.Movement) {
                    Owner.Movement.Initialize(behaviorData);
                }
            }
        }

        // called in Update()
        protected virtual void AnimationUpdate(float dt)
        {
        }

        // called in FixedUpdate()
        protected virtual void PhysicsUpdate(float dt)
        {
        }

#region Events
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(null != _spawnEffect) {
                _spawnEffect.Trigger(OnSpawnComplete);
            } else {
                OnSpawnComplete();
            }

            return false;
        }

        protected virtual void OnSpawnComplete()
        {
            _isAlive = true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(null != _respawnEffect) {
                _respawnEffect.Trigger(OnReSpawnComplete);
            } else {
                OnReSpawnComplete();
            }

            return false;
        }

        protected virtual void OnReSpawnComplete()
        {
            _isAlive = true;
        }

        public override bool OnDeSpawn()
        {
            _isAlive = false;

            if(null != _despawnEffect) {
                _despawnEffect.Trigger(OnDeSpawnComplete);
            } else {
                OnDeSpawnComplete();
            }

            return false;
        }

        protected virtual void OnDeSpawnComplete()
        {
        }

        public override bool OnSetFacing(Vector3 direction)
        {
#if USE_SPINE
            if(null != SpineAnimationHelper) {
                SpineAnimationHelper.SetFacing(direction);
            }
#endif

            if(null != SpriteAnimationHelper) {
                SpriteAnimationHelper.SetFacing(direction);
            }

            if(null != Owner && null != Owner.Model && BehaviorData.AnimateModel) {
                Owner.Model.transform.forward = direction;
            }

            return false;
        }
#endregion

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(!PauseAnimationOnPause) {
                return;
            }

#if USE_SPINE
            if(SpineAnimationHelper != null) {
                SpineAnimationHelper.Pause(PartyParrotManager.Instance.IsPaused);
            }
#endif

            if(Animator != null) {
                Animator.enabled = !PartyParrotManager.Instance.IsPaused;
            }
        }
#endregion
    }
}
