using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="InputData", menuName="pdxpartyparrot/Core/Data/Input Data")]
    [Serializable]
    public class InputData : ScriptableObject
    {
        [SerializeField]
        private string _keyboardAndMouseScheme = "Keyboard&Mouse";

        public string KeyboardAndMouseScheme => _keyboardAndMouseScheme;

        [SerializeField]
        private string _gamepadScheme = "Gamepad";

        public string GamepadScheme => _gamepadScheme;
    }
}
