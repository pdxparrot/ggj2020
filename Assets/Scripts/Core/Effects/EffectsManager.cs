using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects
{
    public sealed class EffectsManager : SingletonBehavior<EffectsManager>
    {
        [SerializeField]
        private bool _enableAnimation = true;

        public bool EnableAnimation => _enableAnimation;

        [SerializeField]
        private bool _enableAudio = true;

        public bool EnableAudio => _enableAudio;

        [SerializeField]
        private bool _enableVFX = true;

        public bool EnableVFX => _enableVFX;

        [SerializeField]
        private bool _enableShakePosition = true;

        public bool EnableShakePosition => _enableShakePosition;

        [SerializeField]
        private bool _enableViewerShake = true;

        public bool EnableViewerShake => _enableViewerShake;

        [SerializeField]
        private bool _enableRumble = true;

        public bool EnableRumble => _enableRumble;

#region Debug
        [SerializeField]
        private bool _enableDebug;

        public bool EnableDebug => _enableDebug;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitDebugMenu();
        }
#endregion

        private void InitDebugMenu()
        {
            DebugMenuNode debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Core.EffectsManager");
            debugMenuNode.RenderContentsAction = () => {
                GUILayout.BeginVertical();
                    _enableDebug = GUILayout.Toggle(_enableDebug, "Enable Debug");

                    _enableAnimation = GUILayout.Toggle(_enableAnimation, "Enable Animation");
                    _enableAudio = GUILayout.Toggle(_enableAudio, "Enable Audio");
                    _enableVFX = GUILayout.Toggle(_enableVFX, "Enable VFX");
                    _enableShakePosition = GUILayout.Toggle(_enableShakePosition, "Enable Shake Position");
                    _enableViewerShake = GUILayout.Toggle(_enableViewerShake, "Enable Viewer Shake");
                    _enableRumble = GUILayout.Toggle(_enableRumble, "Enable Rumble");
                GUILayout.EndVertical();
            };
        }
    }
}
