using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.ggj2020.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
#region Events
        public override void OnStart()
        {
            base.OnStart();

            GameStateManager.Instance.StartLocal(GameManager.Instance.GameGameData.MainGameStatePrefab, state => {
                /*State.MainGameState mainGameState = (State.MainGameState)state;
                foreach(short playerControllerId in GameManager.Instance.PlayerCharacterControllers) {
                    mainGameState.AddPlayerController(playerControllerId);
                }*/
            });
        }
#endregion
    }
}
