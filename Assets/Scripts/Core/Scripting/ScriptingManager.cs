using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting
{
// TODO: this is a pipe dream, but it would be great to have a visual/node-based scripting engine in here for doing custom behaviors
// this manager is step 1 of that dream

// https://www.reddit.com/r/Unity3D/comments/6e92ga/my_visual_scripting_tool_wip/
// http://gram.gs/gramlog/creating-node-based-editor-unity/
// https://github.com/aphex-/BrotherhoodOfNode (MIT)
// https://forum.unity.com/threads/xnode-a-general-purpose-node-editor.502354/
// https://github.com/Siccity/xNode (MIT)

    public sealed class ScriptingManager : SingletonBehavior<ScriptingManager>
    {
// TODO: start script coroutines

        private readonly HashSet<ScriptRunner> _coroutineScripts = new HashSet<ScriptRunner>();
        private readonly HashSet<ScriptRunner> _updateScripts = new HashSet<ScriptRunner>();

        public void Register(ScriptRunner script)
        {
            Debug.Log($"Registering script {script.name}");

            switch(script.Runtime)
            {
            case ScriptRunner.RuntimeType.Coroutine:
                _coroutineScripts.Add(script);
                break;
            case ScriptRunner.RuntimeType.Update:
                _updateScripts.Add(script);
                break;
            }
        }

        public void Unregister(ScriptRunner script)
        {
            Debug.Log($"Unregistering script {script.name}");

            _coroutineScripts.Remove(script);
            _updateScripts.Remove(script);
        }

// TODO: need to control init / starting / stopping of scripts

#region Unity Lifecycle
        private void Update()
        {
            foreach(ScriptRunner script in _updateScripts) {
                script.Step();
            }
        }
#endregion
    }
}
