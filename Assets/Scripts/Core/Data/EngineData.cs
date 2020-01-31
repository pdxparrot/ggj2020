using System;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="EngineData", menuName="pdxpartyparrot/Core/Data/Engine Data")]
    [Serializable]
    public sealed class EngineData : ScriptableObject
    {
        [SerializeField]
        private bool _enableVSync;

        public bool EnableVSync => _enableVSync;

        [SerializeField]
        [Min(-1)]
        [Tooltip("Set to -1 to disable. Ignored if VSync is enabled.")]
        private int _targetFrameRate = -1;

        public int TargetFrameRate => _targetFrameRate;
    }
}
