using pdxpartyparrot.Core.Data.Scripting.Nodes;
using pdxpartyparrot.Core.Editor.Window;

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Scripting
{
    public sealed class ScriptViewNode : Node
    {
        private const string MainStyleSheet = "ScriptEditorWindow/Node";
        private const string NodeLayout = "ScriptEditorWindow/Node";

        private ScriptNodeData _nodeData;

        public ScriptViewNode(ScriptNodeData nodeData) : base()
        {
            _nodeData = nodeData;

            styleSheets.Add(Resources.Load<StyleSheet>(EditorWindow.CoreStyleSheet));
            styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(NodeLayout);
            mainVisualTree.CloneTree(this);

            title = _nodeData.Name;

            InitializeView();

            SetPosition(_nodeData.Position);
        }

        private void InitializeView()
        {
            Label label = new Label
            {
                text = $"ID: {_nodeData.Id}"
            };
            Add(label);
        }
    }
}
