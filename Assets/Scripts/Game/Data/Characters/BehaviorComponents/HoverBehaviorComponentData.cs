using System;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters.BehaviorComponents
{
    [CreateAssetMenu(fileName="HoverBehaviorComponentData", menuName="pdxpartyparrot/Game/Data/Behavior Components/HoverBehaviorComponent Data")]
    [Serializable]
    public class HoverBehaviorComponentData : CharacterBehaviorComponentData
    {
        [SerializeField]
        [Range(0, 10)]
        [Tooltip("How long to hold hover before hovering starts")]
        private float _hoverHoldSeconds = 0.5f;

        public float HoverHoldSeconds => _hoverHoldSeconds;

        [SerializeField]
        [Range(0, 60)]
        [Tooltip("Max time hover can last")]
        private float _hoverTimeSeconds = 10.0f;

        public float HoverTimeSeconds => _hoverTimeSeconds;

        [SerializeField]
        [Range(0, 10)]
        private float _hoverCooldownSeconds = 1.0f;

        public float HoverCooldownSeconds => _hoverCooldownSeconds;

        [SerializeField]
        [Range(0, 10)]
        [Tooltip("Seconds of charge to recover every second after cooldown")]
        private float _hoverRechargeRate = 0.5f;

        public float HoverRechargeRate => _hoverRechargeRate;

        [SerializeField]
        [Range(0, 100)]
        [Tooltip("The acceleration caused by hovering")]
        private float _hoverAcceleration = 5.0f;

        public float HoverAcceleration => _hoverAcceleration;

        [SerializeField]
        [Range(0, 100)]
        private float _hoverMoveSpeed = 30.0f;

        public float HoverMoveSpeed => _hoverMoveSpeed;

        [SerializeField]
        private bool _hoverWhenGrounded;

        public bool HoverWhenGrounded => _hoverWhenGrounded;

        [Space(10)]

#region Animations
        [Header("Animations")]

        [SerializeField]
        private string _hoverParam = "Hover";

        public string HoverParam => _hoverParam;
#endregion
    }
}