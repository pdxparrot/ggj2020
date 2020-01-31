#if ENABLE_SERVER_SPECTATOR && USE_NETWORKING
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Network;

namespace pdxpartyparrot.Game.Camera
{
    public sealed class ServerSpectatorViewer : CinemachineViewer
    {
        public void Initialize(ServerSpectator owner)
        {
            Initialize(0);

            //Follow(owner.FollowTarget);
            LookAt(owner.transform);
        }
    }
}
#endif
