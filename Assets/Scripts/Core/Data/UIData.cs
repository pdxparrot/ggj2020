using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Effects;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="UIData", menuName="pdxpartyparrot/Core/Data/UI Data")]
    [Serializable]
    public class UIData : ScriptableObject
    {
        [SerializeField]
        private LayerMask _uiLayer;

        public LayerMask UILayer => _uiLayer;

        [SerializeField]
        private TMP_FontAsset _defaultFont;

        public TMP_FontAsset DefaultFont => _defaultFont;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _defaultButtonHoverEffectPrefab;

        [CanBeNull]
        public EffectTrigger DefaultButtonHoverEffectTriggerPrefab => _defaultButtonHoverEffectPrefab;

        [SerializeField]
        [FormerlySerializedAs("_defaultButtonClickEffectPrefab")]
        [CanBeNull]
        private EffectTrigger _defaultButtonSubmitEffectPrefab;

        [CanBeNull]
        public EffectTrigger DefaultButtonSubmitEffectTriggerPrefab => _defaultButtonSubmitEffectPrefab;

        [SerializeField]
        [CanBeNull]
        private EffectTrigger _defaultButtonBackEffectPrefab;

        [CanBeNull]
        public EffectTrigger DefaultButtonBackEffectTriggerPrefab => _defaultButtonBackEffectPrefab;
    }
}
