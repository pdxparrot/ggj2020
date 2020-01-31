using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;

namespace pdxpartyparrot.Core.Scripting
{
    public sealed class ScriptContext
    {
        private readonly Dictionary<string, object> _contextData = new Dictionary<string, object>();

        private readonly ScriptRunner _runner;

        public ScriptContext(ScriptRunner runner)
        {
            _runner = runner;
        }

#region Runner Interface
        public void Advance()
        {
            _runner.Advance();
        }

        public void Complete()
        {
            _runner.Complete();
        }

        public void Reset()
        {
            _runner.Reset();
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
