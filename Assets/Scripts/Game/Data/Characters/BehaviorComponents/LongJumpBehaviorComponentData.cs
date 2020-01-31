using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="LongJumpBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/LongJumpBehaviorComponent Data")]
    [Serializable]
    public class LongJumpBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        [Range(0, 10)]
        [Tooltip("How long to hold jump before allowing a long jump")]
        private float _longJumpHoldSeconds = 0.5f;

        public float LongJumpHoldSeconds => _longJumpHoldSeconds;

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("How high does the character jump when long jumping")]
        private float _longJumpHeight = 50.0f;

        public float LongJumpHeight => _longJumpHeight;
    }
}