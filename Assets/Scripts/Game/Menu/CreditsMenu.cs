using pdxpartyparrot.Game.State;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class CreditsMenu : MenuPanel
    {
        [SerializeField]
        private TextMeshProUGUI _creditsText;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _creditsText.richText = true;
            _creditsText.text = GameStateManager.Instance.GameManager.CreditsData.ToString();
        }
#endregion
    }
}
