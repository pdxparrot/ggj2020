using pdxpartyparrot.Core.Data.Actors.Components;
using pdxpartyparrot.ggj2020.Data.Players;

using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        public Player GamePlayerOwner => (Player)Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Player);

            base.Awake();
        }
#endregion

        public override void Initialize(ActorBehaviorComponentData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);
        }
    }
}
