using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    public sealed class GameUI : Game.UI.GameUI
    {
        [SerializeField]
        private PlayerHUD _hud;

        public PlayerHUD HUD => _hud;
    }
}
