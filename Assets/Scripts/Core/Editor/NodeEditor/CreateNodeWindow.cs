using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Util;

using UnityEditor;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor.NodeEditor
{
    public abstract class CreateNodeWindow<D, A, V> : Window.EditorWindow where D: NodeData where A: NodeAttribute where V: NodeEditorNode
    {
        private NodeEditorView _view;

        public NodeEditorView View
        {
            get => _view;

            protected set
            {
                _view = value;

                FilterNodes(string.Empty);
            }
        }

        private readonly List<Type> _nodeTypes = new List<Type>();

        private readonly Dictionary<string, Type> _nodes = new Dictionary<string, Type>();

        private readonly List<string> _filteredNodeNames = new List<string>();

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            ReflectionUtils.FindSubClassesOfInNamespace<D>(_nodeTypes, EditorSettings.projectGenerationRootNamespace);
            foreach(Type nodeType in _nodeTypes) {
                A attr = nodeType.GetCustomAttribute<A>();
                if(null == attr) {
                    Debug.LogWarning($"Node type {nodeType} missing node attribute!");
                    continue;
                }

                _nodes.Add(attr.Name, nodeType);
            }
            //Debug.Log($"Found {_nodes.Count} nodes");
        }
#endregion

        protected string GetNodeName(int index)
        {
            return _filteredNodeNames[index];
        }

        protected abstract bool FilterNode(Type nodeType, A attr);

        // TODO: this could be done much better
        protected void FilterNodes(string filterText)
        {
            _filteredNodeNames.Clear();

            _filteredNodeNames.AddRange(_nodes.Keys.Where(x => {
                if(string.IsNullOrWhiteSpace(x)) {
                    return false;
                }

                Type nodeType = _nodes[x];

                A attr = nodeType.GetCustomAttribute<A>();
                if(null == attr || !FilterNode(nodeType, attr)) {
                    return false;
                }

                return -1 != x.IndexOf(filterText, StringComparison.InvariantCultureIgnoreCase);
            }));

            SetNodeList(_filteredNodeNames);
        }

        protected abstract void SetNodeList(IList nodes);

        protected abstract V CreateViewNode(D nodeData);

        protected V CreateNode(string name, Vector2 position)
        {
            D nodeData = (D)Activator.CreateInstance(_nodes[name], new Rect(position, Vector2.zero));
            return CreateViewNode(nodeData);
        }
    }
}
