using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.NPCs.BehaviorComponents
{
    public abstract class NPCBehaviorComponent : CharacterBehaviorComponent
    {
        protected NPCBehavior NPCBehavior => (NPCBehavior)Behavior;

        public override void Initialize(CharacterBehavior behavior)
        {
            Assert.IsTrue(behavior is NPCBehavior);

            base.Initialize(behavior);
        }
    }
}
