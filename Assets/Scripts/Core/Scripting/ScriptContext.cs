using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Scripting.Nodes;

namespace pdxpartyparrot.Core.Scripting
{
    [Serializable]
    public sealed class ScriptContext
    {
        private readonly Dictionary<string, object> _contextData = new Dictionary<string, object>();

        private readonly ScriptRunner _runner;

        public ScriptContext(ScriptRunner runner)
        {
            _runner = runner;
        }

#region Runner Interface
        public void Advance([CanBeNull] ScriptNode node)
        {
            _runner.Advance(node);
        }

        public void Complete()
        {
            _runner.Stop();
        }

        public void ResetScript()
        {
            _runner.ResetScript();
        }
#endregion

#region Data
        public void Set(string key, object value)
        {
            _contextData[key] = value;
        }

        [CanBeNull]
        public object Get(string key)
        {
            return _contextData.GetOrDefault(key);
        }

        public void Clear()
        {
            _contextData.Clear();
        }
#endregion
    }
}
