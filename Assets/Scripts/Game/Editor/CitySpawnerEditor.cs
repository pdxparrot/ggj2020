using pdxpartyparrot.Game.World;

using UnityEditor;
using UnityEngine;

namespace pdxpartyparrot.Game.Editor
{
    [CustomEditor(typeof(CitySpawner))]
    public class CitySpawnerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            CitySpawner spawner = (CitySpawner)target;
            if(GUILayout.Button("Generate")) {
                spawner.Generate(true);
            }

            if(GUILayout.Button("Reset")) {
                spawner.ResetSpawner(true);
            }
        }
    }
}
