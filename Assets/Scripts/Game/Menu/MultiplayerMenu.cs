using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class MultiplayerMenu : MenuPanel
    {
#region Event Handlers
        // TODO: these methods take in the main game state now
        public void OnHost()
        {
            //GameStateManager.Instance.StartHost();
        }

        public void OnJoin()
        {
            //GameStateManager.Instance.StartJoin();
        }
#endregion
    }
}
