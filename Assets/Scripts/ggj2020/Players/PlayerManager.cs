using pdxpartyparrot.Game.Players;
using pdxpartyparrot.ggj2020.Data.Players;

namespace pdxpartyparrot.ggj2020.Players
{
    public sealed class PlayerManager : PlayerManager<PlayerManager>
    {
        public PlayerData GamePlayerData => (PlayerData)PlayerData;
    }
}
