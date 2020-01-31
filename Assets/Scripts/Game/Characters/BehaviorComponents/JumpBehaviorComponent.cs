using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.BehaviorComponents
{
    public class JumpBehaviorComponent : CharacterBehaviorComponent
    {
#region Actions
        public class JumpAction : CharacterBehaviorAction
        {
            public static JumpAction Default = new JumpAction();
        }
#endregion

        [SerializeField]
        private JumpBehaviorComponentData _data;

        public JumpBehaviorComponentData JumpBehaviorComponentData
        {
            get => _data;
            set => _data = value;
        }

        [Space(10)]

#region Effects
        [Header("Effects")]

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _jumpEffect;
#endregion

#region Actions
        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is JumpAction)) {
                return false;
            }

            if(!Behavior.IsGrounded || Behavior.IsSliding) {
                return false;
            }

            Behavior.CharacterMovement.Jump(_data.JumpHeight);
            if(null != _jumpEffect) {
                _jumpEffect.Trigger();
            }

            return true;
        }
#endregion
    }
}
