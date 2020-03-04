using UnityEngine;
using UnityEngine.UIElements;

namespace pdxpartyparrot.Core.Editor.Window
{
    public abstract class EditorWindow : UnityEditor.EditorWindow
    {
        public const string CoreStyleSheet = "Engine_Core";

/*
        Example setup:
        --------------

        [MenuItem("PDX Party Parrot/Window...")]
        static void Show()
        {
            ComponentFinderWindow window = GetWindow<CustomWindowType>();
            window.Show();
        }
*/

        public abstract string Title { get; }

        public virtual Vector2 MinSize { get; } = new Vector2(250, 50);

#region Unity Lifecycle
        protected virtual void Awake()
        {
            titleContent = new GUIContent(Title);
            minSize = MinSize;
        }

        protected virtual void OnDestroy()
        {
        }

        protected virtual void OnEnable()
        {
            rootVisualElement.styleSheets.Add(Resources.Load<StyleSheet>(CoreStyleSheet));
        }

        protected virtual void OnDisable()
        {
        }

        protected virtual void OnGUI()
        {
        }
#endregion
    }
}
