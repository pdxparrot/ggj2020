using System;
using System.Reflection;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Scripting.Nodes;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.Scripting
{
    public class ScriptRunner : MonoBehaviour
    {
        public enum RuntimeType
        {
            Update,
            Coroutine
        }

        [SerializeField]
        private ScriptData _data;

        [SerializeField]
        private RuntimeType _runtime = RuntimeType.Update;

        public RuntimeType Runtime => _runtime;

        [SerializeField]
        [ReadOnly]
        private bool _complete;

        private readonly ScriptContext _context;

        private ScriptNode _start;
        private ScriptNode _current;

        public ScriptRunner()
        {
            _context = new ScriptContext(this);
        }

// TODO: make initializing scripts more deterministic so it doesn't bog down load

#region Unity Lifecycle
        private void Awake()
        {
            ScriptingManager.Instance.Register(this);

            InitializeNodes();

            Reset();
        }

        private void OnDestroy()
        {
            Complete();
        }
#endregion

        private void InitializeNodes()
        {
            foreach(ScriptNodeData node in _data.Nodes) {
                ScriptNodeAttribute attr = node.GetType().GetCustomAttribute<ScriptNodeAttribute>();
                if(null == attr) {
                    Debug.LogWarning($"Node type {node.GetType()} missing node attribute!");
                    continue;
                }

                // TODO: instantiate, initialize and add the node
            }
        }

#region Script Lifecycle
        public void Reset()
        {
            Debug.Log("Resetting script");

            _complete = false;
            _context.Clear();

            // TODO: set _start and _current to the start node
        }

        public void Advance()
        {
            Debug.Log("Script advancing");
            // TODO: advance the current node
        }

        public void Run()
        {
            Debug.Log("Script running");

            // this shouldn't happen, but just in case
            if(null == _current) {
                Complete();
            }
        }

        public void Complete()
        {
            if(_complete) {
                return;
            }

            Debug.Log("Script complete");
            if(ScriptingManager.HasInstance) {
                ScriptingManager.Instance.Unregister(this);
            }
            _complete = true;
        }
#endregion

        [CanBeNull]
        public ScriptNode GetNode(ScriptNodeId id)
        {
            // TODO
            return null;
        }
    }
}
