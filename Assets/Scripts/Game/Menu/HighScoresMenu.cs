using System;
using System.Collections.Generic;
using System.Text;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class HighScoresMenu : MenuPanel
    {
        [Serializable]
        private struct ExtraColumnEntry
        {
            public string id;

            public TextMeshProUGUI columnText;
        }

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        [Tooltip("Units per-second to scroll")]
        private float _scrollRate = 100.0f;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _rankColumnText;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _nameColumnText;

        [SerializeField]
        private TextMeshProUGUI _scoreColumnText;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _playerCountColumnText;

        [SerializeField]
        private ExtraColumnEntry[] _extraColumns;

        [SerializeField]
        [ReadOnly]
        private float _step;

        [SerializeField]
        [ReadOnly]
        private float _lastScrollAmount;

        [SerializeField]
        [ReadOnly]
        private float _lastVerticalPosition;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            if(null != _rankColumnText) {
                _rankColumnText.text = string.Empty;
            }

            if(null != _nameColumnText) {
                _nameColumnText.text = string.Empty;
            }

            _scoreColumnText.text = string.Empty;

            if(null != _playerCountColumnText) {
                _playerCountColumnText.text = string.Empty;
            }

            foreach(ExtraColumnEntry extraColumn in _extraColumns) {
                extraColumn.columnText.text = string.Empty;
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Dictionary<string, StringBuilder> columns = new Dictionary<string, StringBuilder>();
            HighScoreManager.Instance.HighScoresText(columns);

            if(null != _rankColumnText) {
                _rankColumnText.text = columns.GetOrAdd("rank").ToString();
            }

            if(null != _nameColumnText) {
                _nameColumnText.text = columns.GetOrAdd("name").ToString();
            }

            _scoreColumnText.text = columns.GetOrAdd("score").ToString();

            if(null != _playerCountColumnText) {
                _playerCountColumnText.text = columns.GetOrAdd("playerCount").ToString();
            }

            foreach(ExtraColumnEntry extraColumn in _extraColumns) {
                extraColumn.columnText.text = columns.GetOrAdd(extraColumn.id).ToString();
            }

            _step = Mathf.Clamp01(_scrollRate / _scrollRect.content.rect.height);

            _scrollRect.verticalNormalizedPosition = 1.0f;
            _lastVerticalPosition = _scrollRect.verticalNormalizedPosition;
        }
#endregion

#region Event Handlers
        public override void OnMove(InputAction.CallbackContext context)
        {
            if(!context.performed) {
                return;
            }

            // don't scroll past the bottom
            if(_scrollRect.verticalNormalizedPosition <= 0.0f) {
                _lastScrollAmount = 0.0f;
                _lastVerticalPosition = _scrollRect.verticalNormalizedPosition = 0.0f;
                return;
            }

            // TODO: we're making a huge assumption that input runs in the fixed update here
            // is there a way we can check / validate that assumption?
            _lastScrollAmount = _step * Time.fixedDeltaTime;

            //_scrollRect.verticalNormalizedPosition = Mathf.MoveTowards(_scrollRect.verticalNormalizedPosition, 0.0f, _lastScrollAmount);
            _lastVerticalPosition = _scrollRect.verticalNormalizedPosition;
        }
#endregion
    }
}
