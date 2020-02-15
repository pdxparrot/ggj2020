using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class Menu : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;

        [SerializeField]
        private MenuPanel _mainPanel;

        public MenuPanel MainPanel => _mainPanel;

        [SerializeField]
        [CanBeNull]
        private TextMeshProUGUI _creditTitleText;

        [SerializeField]
        [ReadOnly]
        private MenuPanel _currentPanel;

        public MenuPanel CurrentPanel => _currentPanel;

        private readonly Stack<MenuPanel> _panelStack = new Stack<MenuPanel>();

#region Unity Lifecycle
        private void Awake()
        {
            _canvas.sortingOrder = 100;

            if(null != _creditTitleText) {
                _creditTitleText.text = GameStateManager.Instance.GameManager.CreditsData.GetContributorString();
            }

            PushPanel(_mainPanel);
        }
#endregion

        public void Initialize()
        {
            _mainPanel.Initialize();
        }

        public void ResetMenu()
        {
            _currentPanel.ResetMenu();
        }

        public void PushPanel(MenuPanel panel)
        {
            if(null != _currentPanel) {
                _currentPanel.gameObject.SetActive(false);
                _panelStack.Push(_currentPanel);
            }

            _currentPanel = panel;
            _currentPanel.gameObject.SetActive(true);
            _currentPanel.ResetMenu();
        }

        public void PopPanel()
        {
            if(_panelStack.Count < 1) {
                return;
            }

            _currentPanel.gameObject.SetActive(false);

            _currentPanel = _panelStack.Pop();
            _currentPanel.gameObject.SetActive(true);
        }
    }
}
