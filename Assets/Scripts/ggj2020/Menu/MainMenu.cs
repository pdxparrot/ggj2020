using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.ggj2020.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
#region Events
        public override void OnStart()
        {
            base.OnStart();

            GameStateManager.Instance.StartLocal(GameManager.Instance.GameGameData.MainGameStatePrefab);
        }
#endregion
    }
}
