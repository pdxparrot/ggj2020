using System.Collections.Generic;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    public sealed class LocalizationManager : SingletonBehavior<LocalizationManager>
    {
        [SerializeField]
        private LocalizationData _localizationData;

        private readonly Dictionary<string, string> _stringTable = new Dictionary<string, string>();

#region Unity Lifecycle
        private void Awake()
        {
            // TODO: build the string table
        }
#endregion

        public string GetText(string key)
        {
            return _stringTable.GetOrDefault(key, $"MISSING TEXT '{key}'");
        }
    }
}
