using pdxpartyparrot.Game.Players.Input;
using pdxpartyparrot.ggj2020.Data.Players;

using UnityEngine.Assertions;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class PlayerInputHandler : SideScollerPlayerInputHandler
    {
        private PlayerInputData GamePlayerInputData => (PlayerInputData)PlayerInputData;

        private Player GamePlayer => (Player)Player;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInputData is PlayerInputData);
            Assert.IsTrue(Player is Player);
        }
#endregion
    }
}
