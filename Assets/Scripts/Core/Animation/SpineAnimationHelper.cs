#if USE_SPINE
using System;

using JetBrains.Annotations;

using Spine;
using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.Core.Animation
{
    public class SpineAnimationHelper : MonoBehaviour
    {
        [SerializeField]
        [CanBeNull]
        private SkeletonAnimation _skeletonAnimation;

        [CanBeNull]
        public SkeletonAnimation SkeletonAnimation
        {
            get => _skeletonAnimation;
            set => _skeletonAnimation = value;
        }

        public void Pause(bool pause)
        {
            // null check this just in case we pause
            // before the skeleton link is valid
            if(null != SkeletonAnimation) {
                SkeletonAnimation.timeScale = pause ? 0.0f : 1.0f;
            }
        }

        public void ResetAnimation()
        {
            if(null != SkeletonAnimation) {
                SkeletonAnimation.ClearState();
            }
        }

        [CanBeNull]
        public TrackEntry SetAnimation(string animationName, bool loop)
        {
            return SetAnimation(0, animationName, loop);
        }

        [CanBeNull]
        public TrackEntry SetAnimation(int track, string animationName, bool loop)
        {
            if(null == SkeletonAnimation) {
                return null;
            }

            try {
                return SkeletonAnimation.AnimationState.SetAnimation(track, animationName, loop);
            } catch(Exception e) {
                Debug.LogError($"Exception setting spine animation: {e}");
                return null;
            }
        }

        public void SetEmptyAnimation(int track)
        {
            if(null == SkeletonAnimation) {
                return;
            }

            SkeletonAnimation.AnimationState.SetEmptyAnimation(track, 0.0f);
        }

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Approximately(direction.x, 0.0f)) {
                return;
            }

            // TODO: if the skeleton is scaled, does this unscale it?
            // if so, we might have to take the Abs() first
            if(null != SkeletonAnimation) {
                SkeletonAnimation.Skeleton.ScaleX = Mathf.Sign(direction.x);
            }
        }
    }
}
#endif
