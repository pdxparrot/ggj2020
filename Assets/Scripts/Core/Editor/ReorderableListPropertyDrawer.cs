using UnityEngine;
using UnityEditor;

// https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GUI/ReorderableList.cs
// this is an even more amped up example of this: https://github.com/garettbass/UnityExtensions.ArrayDrawer

namespace pdxpartyparrot.Core.Util.Editor
{
    [CustomPropertyDrawer(typeof(ReorderableListAttribute))]
    public class ReorderableListPropertyDrawer : PropertyDrawer
    {
        private UnityEditorInternal.ReorderableList _reorderableList;

#region Unity Lifecycle
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty listProperty = property.FindPropertyRelative("_items");
            if(null == _reorderableList) {
                Initialize(property, listProperty, new GUIContent(label.text));
            }

            float elementHeight = 0.0f;
            for(int i=0; i<listProperty.arraySize; ++i) {
                elementHeight = Mathf.Max(elementHeight, EditorGUI.GetPropertyHeight(listProperty.GetArrayElementAtIndex(i)));
            }
            _reorderableList.elementHeight = elementHeight;

            _reorderableList.DoList(position);
        }
#endregion

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty listProperty = property.FindPropertyRelative("_items");
            if(null == _reorderableList) {
                Initialize(property, listProperty, new GUIContent(label.text));
            }
            return _reorderableList.GetHeight();
        }

        private void Initialize(SerializedProperty property, SerializedProperty listProperty, GUIContent label)
        {
            _reorderableList = new UnityEditorInternal.ReorderableList(property.serializedObject, listProperty, true, true, true, true)
            {
                drawElementCallback = (position, index, isActive, isFocused) =>
                {
                    position.width -= 40;
                    position.x += 20;
                    EditorGUI.PropertyField(position, listProperty.GetArrayElementAtIndex(index), true);
                },

                drawHeaderCallback = (position) => {
                    EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                },
            };
        }
    }
}
