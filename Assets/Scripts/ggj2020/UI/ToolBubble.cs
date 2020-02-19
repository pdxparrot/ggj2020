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

            //Debug.Log("Showing thumb left bubble");

            pressedButtonAndThumb.SetActive(true);
        }

        public void ShowThumbRight()
        {
            Hide();

            //Debug.Log("Showing thumb right bubble");

            pressedButtonAndThumbRight.SetActive(true);
        }

        public void ShowPressedButton()
        {
            Hide();

            //Debug.Log("Showing pressed button bubble");

            pressedButton.SetActive(true);
        }

        public void ShowUnpressedButton()
        {
            Hide();

            //Debug.Log("Showing unpressed button bubble");

            unpressedButton.SetActive(true);
        }

        public void Hide()
        {
            //Debug.Log("Hiding bubble");

            pressedButton.SetActive(false);
            unpressedButton.SetActive(false);
            pressedButtonAndThumb.SetActive(false);
            pressedButtonAndThumbRight.SetActive(false);
        }
    }
}
