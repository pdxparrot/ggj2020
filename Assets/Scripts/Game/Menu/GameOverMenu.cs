using pdxpartyparrot.Game.State;

namespace pdxpartyparrot.Game.Menu
{
    public abstract class GameOverMenu : MenuPanel
    {
#region Event Handlers
        public virtual void OnDone()
        {
            GameStateManager.Instance.TransitionToInitialStateAsync();
        }
#endregion
    }
}
