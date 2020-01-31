#if USE_SPINE
using pdxpartyparrot.Core.Util;

using Spine;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SpineSkinSwapEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private SpineSkinHelper _skinHelper;

        [SerializeField]
        private string _skinName = "default";

        public override bool WaitForComplete => false;

        private TrackEntry _trackEntry;

        public override void OnStart()
        {
            _skinHelper.Skin = _skinName;
        }
    }
}
#endif
