using JetBrains.Annotations;

using pdxpartyparrot.Core.UI;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Loading
{
    public sealed class LoadingScreen : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private ProgressBar _progressBar;

        public ProgressBar ProgressBar => _progressBar;

        [SerializeField]
        private TextMeshProUGUI _progressText;

        public string ProgressText
        {
            get => _progressText.text;
            set => _progressText.text = value;
        }

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _loadingTips;

#region Unity Lifecycle
        private void Awake()
        {
            _canvas.sortingOrder = 9999;
        }
#endregion

        public void ShowLoadingTip([CanBeNull] string loadingTip)
        {
            if(null == _loadingTips || null == loadingTip) {
                return;
            }

            _loadingTips.text = loadingTip;
        }
    }
}
