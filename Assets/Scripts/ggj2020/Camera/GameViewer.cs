using Cinemachine;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.ggj2020.Data;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.Camera
{
    [RequireComponent(typeof(CinemachineFramingTransposer))]
    [RequireComponent(typeof(CinemachinePOV))]
    [RequireComponent(typeof(CinemachineConfiner))]
    public sealed class GameViewer : CinemachineViewer, IPlayerViewer
    {
        public Viewer Viewer => this;

        public void Initialize(GameData gameData)
        {
            Viewer.Set2D(gameData.ViewportSize);
        }
    }
}
