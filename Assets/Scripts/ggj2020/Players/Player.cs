using pdxpartyparrot.Game.Characters.Players;

using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class Player : Player25D
    {
        public PlayerInput GamePlayerInput => (PlayerInput)PlayerInput;

        public PlayerBehavior GamePlayerBehavior => (PlayerBehavior)PlayerBehavior;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInput is PlayerInput);
            Assert.IsTrue(PlayerBehavior is PlayerBehavior);
        }
#endregion
    }
}
