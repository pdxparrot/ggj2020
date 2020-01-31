using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

// http://www.brechtos.com/tagselectorattribute/

// original by DYLAN ENGELMAN http://jupiterlighthousestudio.com/custom-inspectors-unity/
// altered by Brecht Lecluyse http://www.brechtos.com

namespace pdxpartyparrot.Core.Util.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute), true)]
    public class TagSelectorPropertyDrawer : PropertyDrawer
    {
#region Unity Lifecycle
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(SerializedPropertyType.String != property.propertyType) {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            EditorGUI.BeginProperty(position, label, property);
            try {
                TagSelectorAttribute attrib = (TagSelectorAttribute)attribute;
                if(attrib.UseDefaultTagFieldDrawer) {
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                    return;
                }

                // generate the taglist + custom tags
                var tagList = new List<string> { "<NoTag>" };
                tagList.AddRange(UnityEditorInternal.InternalEditorUtility.tags);

                string propertyString = property.stringValue;

                int index = -1;
                if(propertyString =="") {
                    // the tag is empty
                    index = 0; // first index is the special <notag> entry
                } else {
                    // check if there is an entry that matches the entry and get the index
                    // we skip index 0 as that is a special custom case
                    for(int i=1; i<tagList.Count; ++i) {
                        if(tagList[i] == propertyString) {
                            index = i;
                            break;
                        }
                    }
                }
            
                // draw the popup box with the current selected index
                index = EditorGUI.Popup(position, label.text, index, tagList.ToArray());

                // adjust the actual string value of the property based on the selection
                if(index == 0) {
                    property.stringValue = "";
                } else if(index >= 1) {
                    property.stringValue = tagList[index];
                } else {
                    property.stringValue = "";
                }
            } finally {
                EditorGUI.EndProperty();
            }
        }
#endregion
    }
}
