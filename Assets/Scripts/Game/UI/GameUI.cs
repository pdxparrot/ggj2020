using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    public abstract class GameUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        public virtual void Initialize(UnityEngine.Camera uiCamera)
        {
            _canvas.worldCamera = uiCamera;
        }
    }
}
