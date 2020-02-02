using System;

using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

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
        private LifeCard _lifeCardPrefab;

        [SerializeField]
        private Transform _lifeContainer;

        private LifeCard[] _lifeCards;

        [SerializeField]
        [ReadOnly]
        private int _currentFailure;

#region Unity Lifecycle
        private void Awake()
        {
            GameManager.Instance.RepairFailureEvent += RepairFailureEventHandler;

            _lifeCards = new LifeCard[GameManager.Instance.GameGameData.MaxFailures];
            for(int i=0; i<_lifeCards.Length; ++i) {
                _lifeCards[i] = Instantiate(_lifeCardPrefab, _lifeContainer);
            }
            _currentFailure = 0;
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
                _timerText.text = $"{GameManager.Instance.GameLevelHelper.TimeRemaining:00}";
            }
        }
#endregion

#region Events
        private void RepairFailureEventHandler(object sender, EventArgs args)
        {
            _lifeCards[_currentFailure].Fail();
            _currentFailure++;
        }
#endregion
    }
}
