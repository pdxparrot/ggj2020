using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public sealed class ToolBubble : MonoBehaviour
    {
        [SerializeField]
        private GameObject pressedButton;

        [SerializeField]
        private GameObject unpressedButton;

        [SerializeField]
        private GameObject pressedButtonAndThumb;

        [SerializeField]
        private GameObject pressedButtonAndThumbRight;

        public void ShowThumbLeft()
        {
            Hide();

            pressedButtonAndThumb.SetActive(true);
        }

        public void ShowThumbRight()
        {
            Hide();

            pressedButtonAndThumbRight.SetActive(true);
        }

        public void ShowPressedSprite()
        {
            Hide();

            pressedButton.SetActive(true);
        }

        public void ShowUnpressedSprite()
        {
            Hide();

            unpressedButton.SetActive(true);
        }

        public void Hide()
        {
            pressedButton.SetActive(false);
            unpressedButton.SetActive(false);
            pressedButtonAndThumb.SetActive(false);
            pressedButtonAndThumbRight.SetActive(false);
        }
    }
}
