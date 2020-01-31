using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextHelper : MonoBehaviour
    {
        [SerializeField]
        private string _stringId;

        [SerializeField]
        private TMP_FontAsset _fontOverride;

#region Unity Lifecycle
        private void Awake()
        {
            TextMeshProUGUI text = GetComponent<TextMeshProUGUI>();
            text.font = _fontOverride == null ? UIManager.Instance.Data.DefaultFont : _fontOverride;
            text.text = LocalizationManager.Instance.GetText(_stringId);
        }
#endregion
    }
}
