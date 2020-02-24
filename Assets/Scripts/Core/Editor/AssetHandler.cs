using pdxpartyparrot.Core.Data.Scripting;
using pdxpartyparrot.Core.Editor.Scripting;

using UnityEditor;

namespace pdxpartyparrot.Core.Editor
{
    public static class AssetHandler
    {
        [UnityEditor.Callbacks.OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            string assetPath = AssetDatabase.GetAssetPath(instanceID);

            if(OpenScriptData(assetPath)) {
                return true;
            }

            return false;
        }

        private static bool OpenScriptData(string assetPath)
        {
            ScriptData scriptData = AssetDatabase.LoadAssetAtPath<ScriptData>(assetPath);
            if(null == scriptData) {
                return false;
            }

            ScriptEditorWindow.OpenAsset(scriptData);
            return true;
        }
    }
}
