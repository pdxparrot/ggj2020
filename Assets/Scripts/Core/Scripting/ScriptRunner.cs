using System;
using System.Collections.Generic;
using System.Reflection;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data.NodeEditor;
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
        [CanBeNull]
        private ScriptData _data;

        [SerializeField]
        private RuntimeType _runtime = RuntimeType.Update;

        public RuntimeType Runtime => _runtime;

        [SerializeField]
        private bool _playOnAwake;

        public bool PlayOnAwake
        {
            get => _playOnAwake;
            set => _playOnAwake = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _playing;

        public bool IsPlaying
        {
            get => _playing;
            private set => _playing = value;
        }

        [SerializeReference]
        [ReadOnly]
        private ScriptContext _context;

        private readonly Dictionary<NodeId, ScriptNode> _nodes = new Dictionary<NodeId, ScriptNode>();

        [SerializeReference]
        [CanBeNull]
        private ScriptNode _startNode;

        [SerializeReference]
        [CanBeNull]
        private ScriptNode _currentNode;

// TODO: make initializing scripts more deterministic so it doesn't bog down load

#region Unity Lifecycle
        private void Awake()
        {
            ScriptingManager.Instance.Register(this);

            _context = new ScriptContext(this);

            Initialize();

            if(PlayOnAwake) {
                Play();
            }
        }

        private void OnDestroy()
        {
            Stop();

            if(ScriptingManager.HasInstance) {
                ScriptingManager.Instance.Unregister(this);
            }
        }
#endregion

        public void SetData(ScriptData scriptData)
        {
            Stop();

            _data = scriptData;

            Initialize();
        }

        private void Initialize()
        {
            ResetScript();

            _startNode = null;
            _currentNode = null;

            if(null == _data) {
                return;
            }

            // first pass to create the nodes
            foreach(ScriptNodeData nodeData in _data.Nodes) {
                if(!ValidateNode(nodeData, out ScriptNodeAttribute nodeAttribute, out ScriptNode scriptNode)) {
                    continue;
                }

                scriptNode = (ScriptNode)Activator.CreateInstance(nodeAttribute.ScriptNodeType, nodeData);
                _nodes.Add(nodeData.Id, scriptNode);
            }

            // second pass to initialize them
            foreach(ScriptNode node in _nodes.Values) {
                node.Initialize(this);
            }
        }

        private bool ValidateNode(ScriptNodeData nodeData, out ScriptNodeAttribute nodeAttribute, out ScriptNode scriptNode)
        {
            nodeAttribute = null;
            scriptNode = null;

            if(!nodeData.Id.IsValid) {
                Debug.LogWarning($"Invalid node of type {nodeData.GetType()}!");
                return false;
            }

            bool isStartNode = nodeData.GetType() == typeof(StartNodeData);
            if(isStartNode && null != _startNode) {
                Debug.LogWarning($"Duplicate start nodes found: {_startNode.Id} and {nodeData.Id}!");
                return false;
            }

            nodeAttribute = nodeData.GetType().GetCustomAttribute<ScriptNodeAttribute>();
            if(null == nodeAttribute) {
                Debug.LogWarning($"Node type {nodeData.GetType()} missing node attribute!");
                return false;
            }

            scriptNode = _nodes.GetOrDefault(nodeData.Id);
            if(null != scriptNode) {
                Debug.LogWarning($"Duplicate node {nodeData.Id} (one of type {nodeData.GetType()} and the other of type {scriptNode.GetType()})!");
                return false;
            }

            return true;
        }

#region Script Lifecycle
        public void ResetScript()
        {
            Stop();

            Debug.Log("Resetting script");

            IsPlaying = false;
            _context.Clear();

            _currentNode = _startNode;
        }

        internal void Advance([CanBeNull] ScriptNode node)
        {
            Debug.Log("Script advancing");

            _currentNode = node;
        }

        public void Play()
        {
            if(IsPlaying) {
                return;
            }

            Debug.Log("Script playing");
            IsPlaying = true;
        }

        internal void Step()
        {
            if(!IsPlaying) {
                return;
            }

            if(null == _currentNode) {
                Stop();
                return;
            }

            _currentNode.Run(_context);
        }

        public void Stop()
        {
            if(!IsPlaying) {
                return;
            }

            Debug.Log("Script stopping");
            IsPlaying = false;
        }
#endregion

        [CanBeNull]
        internal ScriptNode GetNode(NodeId id)
        {
            return _nodes.GetOrDefault(id);
        }
    }
}
