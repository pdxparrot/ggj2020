﻿using pdxpartyparrot.Core.Util;

using UnityEditor;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor
{
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public sealed class ReadOnlyPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
 
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
                EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}
