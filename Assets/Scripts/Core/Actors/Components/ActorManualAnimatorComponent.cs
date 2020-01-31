using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Core.Actors.Components
{
    public class ActorManualAnimatorComponent : ActorComponent
    {
        [Serializable]
        protected struct AnimationState
        {
            public bool IsAnimating;

            public float AnimationSeconds;
            public float AnimationSecondsRemaining;

            public float PercentComplete => 1.0f - (AnimationSecondsRemaining / AnimationSeconds);

            public bool IsFinished => AnimationSecondsRemaining <= 0.0f;

            public Vector3 StartPosition;
            public Vector3 EndPosition;

            public Quaternion StartRotation;
            public Quaternion EndRotation;

            public bool IsKinematic;

            public Action OnComplete;
        }

        [SerializeField]
        [ReadOnly]
        protected AnimationState _animationState;

        public virtual bool IsAnimating => _animationState.IsAnimating;

        public override bool CanMove => !IsAnimating;

#region Unity Lifecycle
        protected void Update()
        {
            float dt = UnityEngine.Time.deltaTime;

            UpdateAnimation(dt);
        }
#endregion

        public virtual void StartAnimation(Vector3 targetPosition, Quaternion targetRotation, float timeSeconds, Action onComplete=null)
        {
            if(IsAnimating || null == Owner || null == Owner.Movement) {
                return;
            }

            if(ActorManager.Instance.EnableDebug) {
                Debug.Log($"Starting manual animation from {Owner.Movement.Position}:{Owner.Movement.Rotation} to {targetPosition}:{targetRotation} over {timeSeconds} seconds");
            }

            _animationState.IsAnimating = true;

            _animationState.StartPosition = Owner.Movement.Position;
            _animationState.EndPosition = targetPosition;

            _animationState.StartRotation = Owner.Movement.Rotation;
            _animationState.EndRotation = targetRotation;

            _animationState.AnimationSeconds = timeSeconds;
            _animationState.AnimationSecondsRemaining = timeSeconds;

            _animationState.IsKinematic = Owner.Movement.IsKinematic;
            Owner.Movement.IsKinematic = true;

            _animationState.OnComplete = onComplete;
        }

        protected virtual void UpdateAnimation(float dt)
        {
            if(!IsAnimating || PartyParrotManager.Instance.IsPaused || null == Owner || null == Owner.Movement) {
                return;
            }

            Profiler.BeginSample("ActorAnimator.UpdateAnimation");
            try {
                if(_animationState.IsFinished) {
                    if(ActorManager.Instance.EnableDebug) {
                        Debug.Log("Actor animation complete!");
                    }

                    _animationState.IsAnimating = false;

                    Owner.Movement.Position = _animationState.EndPosition;
                    Owner.Movement.Rotation = _animationState.EndRotation;
                    Owner.Movement.IsKinematic = _animationState.IsKinematic;

                    _animationState.OnComplete?.Invoke();
                    _animationState.OnComplete = null;

                    return;
                }

                _animationState.AnimationSecondsRemaining -= dt;
                if(_animationState.AnimationSecondsRemaining < 0.0f) {
                    _animationState.AnimationSecondsRemaining = 0.0f;
                }

                Owner.Movement.Position = Vector3.Slerp(_animationState.StartPosition, _animationState.EndPosition, _animationState.PercentComplete);
                Owner.Movement.Rotation = Quaternion.Slerp(_animationState.StartRotation, _animationState.EndRotation, _animationState.PercentComplete);
            } finally {
                Profiler.EndSample();
            }
        }
    }
}
