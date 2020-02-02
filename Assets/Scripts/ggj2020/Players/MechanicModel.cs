using pdxpartyparrot.Core.Actors.Components;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Players
{
    public class MechanicModel : MonoBehaviour
    {
        public void InitializeBehavior(ActorBehaviorComponent behavior, short playerControllerId)
        {
            behavior.SpineSkinHelper.SetSkin(playerControllerId);
        }
    }
}
