using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="DashBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/DashBehaviorComponent Data")]
    [Serializable]
    public class DashBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        [Range(0.0f, 5.0f)]
        [Tooltip("How long the dash lasts")]
        private float _dashTimeSeconds = 1.0f;

        public float DashTimeSeconds => _dashTimeSeconds;

        [SerializeField]
        [Range(0, 10)]
        private float _dashCooldownSeconds = 1.0f;

        public float DashCooldownSeconds => _dashCooldownSeconds;

        [SerializeField]
        [Range(0, 10)]
        private float _dashDistance = 1.0f;

        public float DashDistance => _dashDistance;

        public float DashSpeed => DashDistance / DashTimeSeconds;

        [SerializeField]
        private bool _disableGravity = true;

        public bool DisableGravity => _disableGravity;
    }
}