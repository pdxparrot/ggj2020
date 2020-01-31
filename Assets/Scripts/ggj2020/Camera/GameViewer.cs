using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.ggj2020.Data;

namespace pdxpartyparrot.ggj2020.Camera
{
    public sealed class GameViewer : CinemachineViewer, IPlayerViewer
    {
        public Viewer Viewer => this;

        public void Initialize(GameData gameData)
        {
            Viewer.Set2D(gameData.ViewportSize);
        }
    }
}
