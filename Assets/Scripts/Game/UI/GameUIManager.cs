using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.UI
{
    public interface IGameUIManager
    {
        string DefaultFloatingTextPoolName { get; }

        void Initialize();

        void Shutdown();

        TV InstantiateUIPrefab<TV>(TV prefab) where TV: Component;

        FloatingText InstantiateFloatingText(string poolName);
    }

    public abstract class GameUIManager<T> : SingletonBehavior<T>, IGameUIManager where T: GameUIManager<T>
    {
#region UI / Menus
        [SerializeField]
        private PlayerUI _playerUIPrefab;

        [CanBeNull]
        private PlayerUI _playerUI;

        [CanBeNull]
        public PlayerUI PlayerUI => _playerUI;

        [SerializeField]
        private Menu.Menu _pauseMenuPrefab;

        [CanBeNull]
        private Menu.Menu _pauseMenu;
#endregion

#region Floating Text
        [SerializeField]
        private string _defaultFloatingTextPoolName = "floating_text";

        public string DefaultFloatingTextPoolName => _defaultFloatingTextPoolName;
#endregion

        private GameObject _uiContainer;

        private GameObject _floatingTextContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _uiContainer = new GameObject("UI");
            _floatingTextContainer = new GameObject("Floating Text");

            PartyParrotManager.Instance.PauseEvent += PauseEventHandler;

            GameStateManager.Instance.RegisterGameUIManager(this);
        }

        protected override void OnDestroy()
        {
            if(GameStateManager.HasInstance) {
                GameStateManager.Instance.UnregisterGameUIManager();
            }

            if(PartyParrotManager.HasInstance) {
                PartyParrotManager.Instance.PauseEvent -= PauseEventHandler;
            }

            Destroy(_floatingTextContainer);
            _floatingTextContainer = null;

            Destroy(_uiContainer);
            _uiContainer = null;

            base.OnDestroy();
        }
#endregion

        public void Initialize()
        {
            _pauseMenu = InstantiateUIPrefab(_pauseMenuPrefab);
            if(null != _pauseMenu) {
                _pauseMenu.gameObject.SetActive(PartyParrotManager.Instance.IsPaused);
            }
        }

        public void InitializePlayerUI(UnityEngine.Camera camera)
        {
            Debug.Log("Initializing player UI...");

            _playerUI = InstantiateUIPrefab(_playerUIPrefab);
            if(null != _playerUI) {
                _playerUI.Initialize(camera);
            }
        }

        public void Shutdown()
        {
            if(null != _playerUI) {
                Destroy(_playerUI.gameObject);
            }
            _playerUI = null;

            if(null != _pauseMenu) {
                Destroy(_pauseMenu.gameObject);
            }
            _pauseMenu = null;
        }

        // helper for instantiating UI prefabs under the UI container
        public TV InstantiateUIPrefab<TV>(TV prefab) where TV: Component
        {
            return null == prefab ? null : Instantiate(prefab, _uiContainer.transform);
        }

        // helper for instantiating floating text under the floating text container
        public FloatingText InstantiateFloatingText(string poolName)
        {
            return ObjectPoolManager.Instance.GetPooledObject<FloatingText>(poolName, _floatingTextContainer.transform);
        }

#region Event Handlers
        private void PauseEventHandler(object sender, EventArgs args)
        {
            if(null == _pauseMenu) {
                return;
            }

            if(PartyParrotManager.Instance.IsPaused) {
                _pauseMenu.gameObject.SetActive(true);
                _pauseMenu.ResetMenu();

                if(InputManager.Instance.EnableDebug) {
                    Debug.Log("Enabling UIModule actions");
                }
                InputManager.Instance.EventSystem.UIModule.EnableAllActions();
            } else {
                if(InputManager.Instance.EnableDebug) {
                    Debug.Log("Disabling UIModule actions");
                }
                InputManager.Instance.EventSystem.UIModule.DisableAllActions();

                _pauseMenu.gameObject.SetActive(false);
            }
        }
#endregion
    }
}
