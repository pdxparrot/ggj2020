using pdxpartyparrot.Core.Actors.Components;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    public class MechanicModel : MonoBehaviour
    {
        private ActorBehaviorComponent localBehavior = null;
        public void InitializeBehavior(ActorBehaviorComponent behavior, short playerControllerId)
        {
            behavior.SpineSkinHelper.SetSkin(playerControllerId);
            localBehavior = behavior;
        }

        public void SetAttachment(string slotName, string attachmentName)
        {
            localBehavior.SpineSkinHelper.SetAttachment(slotName, attachmentName);
        }

        public void RemoveAttachment(string slotName)
        {
            localBehavior.SpineSkinHelper.RemoveAttachment(slotName);
        }
    }
}
