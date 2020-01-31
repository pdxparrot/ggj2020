using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    public sealed class InitialInput : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        private TextBlink _blink;

        [SerializeField]
        private GameObject _underscore;

        [SerializeField]
        [ReadOnly]
        private int _currentCharacterIdx;

        public char[] Characters { get; set; }

        public char CurrentCharacter => Characters[_currentCharacterIdx];

#region Unity Lifecycle
        private void Awake()
        {
            _underscore.SetActive(false);
        }
#endregion

        public void Select(bool selected)
        {
            if(selected) {
                _blink.StartBlink();
                _underscore.SetActive(true);
            } else {
                _underscore.SetActive(false);
                _blink.StopBlink();
            }
        }

        public void NextLetter()
        {
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx + 1, Characters.Length);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }

        public void PreviousLetter()
        {
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx - 1, Characters.Length);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }
    }
}
