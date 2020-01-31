#if !USE_NETWORKING
using UnityEngine;

namespace pdxpartyparrot.Core.Network
{
    public class NetworkBehaviour : MonoBehaviour
    {
        public bool isLocalPlayer => true;

        // TODO: find a better value for this
        // (every player should not be player 0)
        public short playerControllerId => 0;
    }
}
#endif
