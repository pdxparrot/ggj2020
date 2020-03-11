using pdxpartyparrot.Core.Editor.NodeEditor;
using pdxpartyparrot.Game.Data;

using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Game.Editor.Dialogue
{
    public sealed class DialogueEditorWindow : NodeEditorWindow
    {
        private const string MainStyleSheet = "DialogueEditorWindow/Main";
        private const string WindowLayout = "DialogueEditorWindow/Window";

        [MenuItem("PDX Party Parrot/Game/Dialogue Editor...")]
        public static void ShowWindow()
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.CreateNewDialogue();
            window.Show();
        }

        public static void OpenAsset(DialogueData dialogueData)
        {
            DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
            window.LoadDialogue(dialogueData);
            window.Show();
        }

        public override string Title => "Dialogue Editor";

        private DialogueView DialogueView => (DialogueView)View;

        private DialogueData _dialogueData;

#region Unity Lifecycle
        protected override void OnEnable()
        {
            base.OnEnable();

            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(MainStyleSheet));

            VisualTreeAsset mainVisualTree = Resources.Load<VisualTreeAsset>(WindowLayout);
            mainVisualTree.CloneTree(rootVisualElement);

            CreateNodeView();
        }
#endregion

        protected override NodeEditorView CreateView()
        {
            return new DialogueView(this);
        }

        protected override IEdgeConnectorListener CreateEdgeConnectorListener()
        {
            return new EdgeConnectorListener(DialogueView);
        }

        private void CreateNewDialogue()
        {
            Debug.Log("Creating new dialogue...");

            _dialogueData = CreateInstance<DialogueData>();
            _dialogueData.name = "DialogueData";
            DialogueView.LoadDialogue(_dialogueData);
        }

        private void LoadDialogue(DialogueData dialogueData)
        {
            Debug.Log($"Loading script {dialogueData.name}...");

            _dialogueData = dialogueData;
            DialogueView.LoadDialogue(_dialogueData);
        }
    }
}
