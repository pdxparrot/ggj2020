using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Billboard))]
    public sealed class UIBubble : MonoBehaviour
    {
        [SerializeField]
        private Sprite pressedButton;

        [SerializeField]
        private Sprite unpressedButton;

        [SerializeField]
        private Sprite pressedButtonAndThumb;

        [SerializeField]
        private Sprite pressedButtonAndThumbRight;

        // I believe this is hooked up because
        // we need it earlier than awake
        [SerializeField]
        private SpriteRenderer _renderer;

        public void SetThumbLeft()
        {
            _renderer.sprite = pressedButtonAndThumb;
        }

        public void SetThumbRight()
        {
            _renderer.sprite = pressedButtonAndThumbRight;
        }

        public void SetPressedSprite()
        {
            _renderer.sprite = pressedButton;
        }

        public void SetUnpressedSprite()
        {
            _renderer.sprite = unpressedButton;
        }

        public void HideSprite()
        {
            _renderer.sprite = null;
        }
    }
}
