using pdxpartyparrot.Core.Camera;

namespace pdxpartyparrot.Game.Camera
{
    public class StaticViewer : Viewer, IPlayerViewer
    {
        public Viewer Viewer => this;
    }
}
