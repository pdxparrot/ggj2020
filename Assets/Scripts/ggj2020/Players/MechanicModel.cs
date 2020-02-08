using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class MechanicModel : MonoBehaviour
    {
        [SerializeField]
        private Player _owner;

        public void Initialize(short playerControllerId)
        {
            _owner.Behavior.SpineSkinHelper.SetSkin(playerControllerId);
        }

        public void SetAttachment(string slotName, string attachmentName)
        {
            _owner.Behavior.SpineSkinHelper.SetAttachment(slotName, attachmentName);
        }

        public void RemoveAttachment(string slotName)
        {
            _owner.Behavior.SpineSkinHelper.RemoveAttachment(slotName);
        }
    }
}
