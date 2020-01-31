using System.IO;

using UnityEditor;
using UnityEngine;

namespace pdxpartyparrot.Core.Editor.Project
{
    public class ProjectManifest
    {
        public const string FileName = ".pdxpartyparrot_project.json";

        [SerializeField]
        private int _version;

        public int Version
        {
            get => _version;
            set => _version = value;
        }

        [SerializeField]
        private bool _useSpine;

        public bool UseSpine
        {
            get => _useSpine;
            set => _useSpine = value;
        }

        [SerializeField]
        private bool _useDOTween;

        public bool UseDOTween
        {
            get => _useDOTween;
            set => _useDOTween = value;
        }

        [SerializeField]
        private bool _useNetworking;

        public bool UseNetworking
        {
            get => _useNetworking;
            set => _useNetworking = value;
        }

        public void Read()
        {
            string content;
            using(StreamReader reader = File.OpenText(FileName)) {
                // TODO: we should make sure the file isn't too big first
                content = reader.ReadToEnd();
            }

            EditorJsonUtility.FromJsonOverwrite(content, this);
        }

        public void Write()
        {
            string manifestJson = EditorJsonUtility.ToJson(this);
            File.WriteAllText(FileName, manifestJson);
        }
    }
}
