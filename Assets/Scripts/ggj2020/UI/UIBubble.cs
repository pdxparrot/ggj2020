using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public class UIBubble : MonoBehaviour
    {
        public Sprite pressedButton;
        public Sprite unpressedButton;
        public Sprite pressedButtonAndThumb;
        public Sprite pressedButtonAndThumbRight;
        private Sprite currentSprite;

        [SerializeField]
        private SpriteRenderer SPRR;

        public void SetThumbLeft()
        {
            SPRR.sprite = pressedButtonAndThumb;
        }

        public void SetThumbRight()
        {
            SPRR.sprite = pressedButtonAndThumbRight;
        }

        public void SetPressedSprite()
        {
            SPRR.sprite = pressedButton;
        }

        public void SetUnpressedSprite()
        {
            SPRR.sprite = unpressedButton;
        }

        public void HideSprite()
        {
            SPRR.sprite = null;
        }
    }
}
