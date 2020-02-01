using pdxpartyparrot.Core.UI;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.ggj2020.UI
{
    [RequireComponent(typeof(UIObject))]
    public sealed class PlayerHUD : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _timerText;

#region Unity Lifecycle
        private void Update()
        {
            if(GameManager.HasInstance && null != GameManager.Instance.GameLevelHelper) {
                _timerText.text = $"{GameManager.Instance.GameLevelHelper.TimeRemaining}";
            }
        }
#endregion
    }
}
