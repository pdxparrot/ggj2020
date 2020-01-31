using pdxpartyparrot.Game.Data;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class CreditsMenu : MenuPanel
    {
        [SerializeField]
        private CreditsData _creditsData;

        [SerializeField]
        private TextMeshProUGUI _creditsText;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _creditsText.richText = true;
            _creditsText.text = _creditsData.ToString();
        }
#endregion
    }
}
