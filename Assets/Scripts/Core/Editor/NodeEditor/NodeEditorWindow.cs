using JetBrains.Annotations;

using pdxpartyparrot.Core.Editor.Window;

using UnityEditor.Experimental.GraphView;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.NodeEditor
{
    // https://forum.unity.com/threads/bare-bones-graphview-example.778706/
    // https://forum.unity.com/threads/how-to-use-the-new-graphview-uielements.536563/
    // https://github.com/Unity-Technologies/ShaderGraph/blob/master/com.unity.shadergraph/Editor/Drawing/Views/
    public abstract class NodeEditorWindow : EditorWindow
    {
        [CanBeNull]
        public NodeEditorView View { get; private set; }

        [CanBeNull]
        public IEdgeConnectorListener EdgeConnectorListener { get; private set; }

        protected abstract NodeEditorView CreateView();

        protected abstract IEdgeConnectorListener CreateEdgeConnectorListener();

        protected virtual void CreateNodeView()
        {
            Assert.IsNull(View);
            View = CreateView();

            rootVisualElement.Clear();
            rootVisualElement.Add(View);
            View.StretchToParentSize();

            View.AddManipulator(new ContentDragger());
            View.AddManipulator(new SelectionDragger());
            View.AddManipulator(new RectangleSelector());
            View.AddManipulator(new ClickSelector());

            EdgeConnectorListener = CreateEdgeConnectorListener();
        }
    }
}
