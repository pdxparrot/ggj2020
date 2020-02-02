using JetBrains.Annotations;

using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public abstract class GameOverMenu : MenuPanel
    {
        [SerializeField]
        [CanBeNull]
        private InitialInputMenu _initialInputMenu;

        [CanBeNull]
        protected InitialInputMenu InitialInputMenu => _initialInputMenu;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            if(null != _initialInputMenu) {
                _initialInputMenu.gameObject.SetActive(false);
            }
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            if(null != _initialInputMenu) {
                Owner.PushPanel(_initialInputMenu);
                _initialInputMenu.Initialize();
            }
        }

#region Event Handlers
        public virtual void OnDone()
        {
            GameStateManager.Instance.TransitionToInitialStateAsync();
        }
#endregion
    }
}
