#if USE_SPINE
using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.Core.Util
{
    public sealed class SpineSkinHelper : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation _skeletonAnimation;

        public SkeletonAnimation SkeletonAnimation
        {
            get => _skeletonAnimation;
            set => _skeletonAnimation = value;
        }

        [SerializeField]
        private string[] _skinNames;

        public Color Color
        {
            get => SkeletonAnimation.Skeleton.GetColor();
            set => SkeletonAnimation.Skeleton.SetColor(value);
        }

        public float Red
        {
            get => SkeletonAnimation.Skeleton.R;
            set => SkeletonAnimation.Skeleton.R = value;
        }

        public float Green
        {
            get => SkeletonAnimation.Skeleton.G;
            set => SkeletonAnimation.Skeleton.G = value;
        }

        public float Blue
        {
            get => SkeletonAnimation.Skeleton.B;
            set => SkeletonAnimation.Skeleton.B = value;
        }

        public float Alpha
        {
            get => SkeletonAnimation.Skeleton.A;
            set => SkeletonAnimation.Skeleton.A = value;
        }

        public string Skin
        {
            get => SkeletonAnimation.Skeleton.Skin.Name;
            set {
                SkeletonAnimation.Skeleton.SetSkin(value);
                SkeletonAnimation.Skeleton.SetSlotsToSetupPose();
                SkeletonAnimation.AnimationState.Apply(SkeletonAnimation.Skeleton);
            }
        }

        public void SetSkin(int index)
        {
            if(index < 0 || index >= _skinNames.Length) {
                Debug.LogWarning($"Attempted to set invalid skin {index} ({_skinNames.Length} total)");
                return;
            }
            Skin = _skinNames[index];
        }

        public void SetRandomSkin()
        {
            if(_skinNames.Length < 1) {
                return;
            }
            SetSkin(PartyParrotManager.Instance.Random.Next(_skinNames.Length));
        }

        public void SetAttachment(string slotName, string attachmentName)
        {
            SkeletonAnimation.Skeleton.SetAttachment(slotName, attachmentName);
        }

        public void RemoveAttachment(string slotName)
        {
            SkeletonAnimation.Skeleton.SetAttachment(slotName, null);
        }
    }
}
#endif
