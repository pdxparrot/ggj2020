using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="DoubleJumpBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/DoubleJumpBehaviorComponent Data")]
    [Serializable]
    public class DoubleJumpBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        [Range(0, 100)]
        [Tooltip("How high does the character jump when double jumping")]
        private float _doubleJumpHeight = 25.0f;

        public float DoubleJumpHeight => _doubleJumpHeight;

        [SerializeField]
        [Tooltip("How many times is the player able to double jump (-1 is infinite)")]
        private int _doubleJumpCount = 1;

        public int DoubleJumpCount => _doubleJumpCount;
    }
}