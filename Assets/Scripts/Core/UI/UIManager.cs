using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        private UIData _data;

        public UIData Data => _data;

        [SerializeField]
        private bool _enableDebug;

#region Default Button Effects
        [CanBeNull]
        public EffectTrigger DefaultButtonHoverEffectTrigger { get; private set; }

        [CanBeNull]
        public EffectTrigger DefaultButtonSubmitEffectTrigger { get; private set; }

        [CanBeNull]
        public EffectTrigger DefaultButtonBackEffectTrigger { get; private set; }
#endregion

        private readonly Dictionary<string, UIObject> _uiObjects = new Dictionary<string, UIObject>();

#region Unity Lifecycle
        private void Awake()
        {
            InitializeDefaultButtonEffects();

            InitDebugMenu();
        }
#endregion

        private void InitializeDefaultButtonEffects()
        {
            if(null != _data.DefaultButtonHoverEffectTriggerPrefab) {
                DefaultButtonHoverEffectTrigger = Instantiate(_data.DefaultButtonHoverEffectTriggerPrefab, transform);
            }

            if(null != _data.DefaultButtonSubmitEffectTriggerPrefab) {
                DefaultButtonSubmitEffectTrigger = Instantiate(_data.DefaultButtonSubmitEffectTriggerPrefab, transform);
            }

            if(null != _data.DefaultButtonBackEffectTriggerPrefab) {
                DefaultButtonBackEffectTrigger = Instantiate(_data.DefaultButtonBackEffectTriggerPrefab, transform);
            }
        }

        [CanBeNull]
        public EffectTrigger GetDefaultButtonClickEffectTrigger(bool isBackButton)
        {
            return isBackButton ? DefaultButtonBackEffectTrigger : DefaultButtonSubmitEffectTrigger;
        }

#region UI Objects
        public void RegisterUIObject(UIObject uiObject)
        {
            if(_enableDebug) {
                Debug.Log($"Registering UI object {uiObject.Id}: {uiObject.name}");
            }

            try {
                _uiObjects.Add(uiObject.Id, uiObject);
            } catch(ArgumentException) {
                Debug.LogWarning($"Failed overwrite of UI object {uiObject.Id}!");
            }
        }

        public bool UnregisterUIObject(UIObject uiObject)
        {
            if(_enableDebug) {
                Debug.Log($"Unregistering UI object {uiObject.Id}");
            }

            return _uiObjects.Remove(uiObject.Id);
        }

        public void ShowUIObject(string id, bool show)
        {
            if(!_uiObjects.TryGetValue(id, out var uiObject)) {
                Debug.LogWarning($"Failed to lookup UI object {id}!");
                return;
            }

            if(_enableDebug) {
                Debug.Log($"Showing UI object {name}: {show}");
            }

            uiObject.gameObject.SetActive(show);
        }
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.UIManager");
            debugMenuNode.RenderContentsAction = () => {
                _enableDebug = GUILayout.Toggle(_enableDebug, "Enable Debug");

                GUILayout.BeginVertical("UI Objects:", GUI.skin.box);
                    foreach(var kvp in _uiObjects) {
                        GUILayout.Label(kvp.Key);
                    }
                GUILayout.EndVertical();
            };
        }
    }
}
