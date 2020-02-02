using System;

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

        [SerializeField]
        private GameObject _lifeCardPrefab;

        [SerializeField]
        private Transform _lifeContainer;

#region Unity Lifecycle
        private void Awake()
        {
            GameManager.Instance.RepairFailureEvent += RepairFailureEventHandler;
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.RepairFailureEvent -= RepairFailureEventHandler;
            }
        }

        private void Update()
        {
            if(GameManager.HasInstance && null != GameManager.Instance.GameLevelHelper) {
                _timerText.text = $"{GameManager.Instance.GameLevelHelper.TimeRemaining:D2}";
            }
        }
#endregion

#region Events
        private void RepairFailureEventHandler(object sender, EventArgs args)
        {
            Instantiate(_lifeCardPrefab, _lifeContainer);
        }
#endregion
    }
}
