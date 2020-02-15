using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="LoadingTipData", menuName="pdxpartyparrot/Core/Data/Loading Tip Data")]
    [Serializable]
    public sealed class LoadingTipData : ScriptableObject
    {
        [SerializeField]
        private int _loadingTipRotateSeconds = 30;

        public int LoadingTipRotateSeconds => _loadingTipRotateSeconds;

        [SerializeField]
        private string[] _loadingTips;

        [CanBeNull]
        public string GetRandomLoadingTip()
        {
            return _loadingTips.GetRandomEntry();
        }
    }
}
