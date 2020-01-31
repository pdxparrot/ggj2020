using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    public abstract class PlayerBehaviorComponent : CharacterBehaviorComponent
    {
        protected PlayerBehavior PlayerBehavior => (PlayerBehavior)Behavior;

        public override void Initialize(CharacterBehavior behavior)
        {
            Assert.IsTrue(behavior is PlayerBehavior);

            base.Initialize(behavior);
        }
    }
}
