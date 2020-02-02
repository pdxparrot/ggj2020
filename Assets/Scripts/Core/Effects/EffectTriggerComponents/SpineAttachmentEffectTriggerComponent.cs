#if USE_SPINE
using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class SpineAttachmentEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private SpineSkinHelper _skinHelper;

        [SerializeField]
        private string _slotName = "default";

        [CanBeNull]
        [SerializeField]
        private string _attachmentName;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            _skinHelper.SetAttachment(_slotName, _attachmentName);
        }
    }
}
#endif
