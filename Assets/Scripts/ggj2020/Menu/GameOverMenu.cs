using pdxpartyparrot.Game;
using pdxpartyparrot.ggj2020.Players;

namespace pdxpartyparrot.ggj2020.Menu
{
    public sealed class GameOverMenu : Game.Menu.GameOverMenu
    {
#region Event Handlers
        public override void OnDone()
        {
            HighScoreManager.Instance.AddHighScore(InitialInputMenu.GetInitials(), GameManager.Instance.RepairSuccesses, PlayerManager.Instance.PlayerCount);

            GameManager.Instance.TransitionToHighScores = true;

            base.OnDone();
        }
#endregion
    }
}
