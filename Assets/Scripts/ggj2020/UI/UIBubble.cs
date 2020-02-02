using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using pdxpartyparrot.ggj2020.UI;

namespace pdxpartyparrot.ggj2020.UI
{
    public class UIBubble : MonoBehaviour
    {
        public Sprite pressedButton;
        public Sprite unpressedButton;
        public Sprite pressedButtonAndThumb;
        public Sprite pressedButtonAndThumbRight;
        private Sprite currentSprite;
        private SpriteRenderer SPRR;

        // Start is called before the first frame update
        void Start()
        {
            SPRR = gameObject.GetComponent<SpriteRenderer>();
        }

        // Update is called once per frame
        void Update()
        {

        }

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
