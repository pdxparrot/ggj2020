using System;

using pdxpartyparrot.Core.Data.NodeEditor;
using pdxpartyparrot.Core.Scripting.Nodes;

using UnityEngine;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScriptNodeAttribute : NodeAttribute
    {
        public enum AllowedInstances
        {
            Single,
            Multiple,
        }

        public AllowedInstances Instances { get; private set; }

        public Type ScriptNodeType { get; private set; }

        public bool AllowMultiple => AllowedInstances.Multiple == Instances;

        public ScriptNodeAttribute(string name, Type scriptNodeType) : base(name)
        {
            if(scriptNodeType.IsSubclassOf(typeof(ScriptNode))) {
                ScriptNodeType = scriptNodeType;
            } else {
                Debug.LogWarning("Script node type must inherit from ScriptNode!");
            }
            Instances = AllowedInstances.Multiple;
        }

        public ScriptNodeAttribute(string name, Type scriptNodeType, AllowedInstances instances) : this(name, scriptNodeType)
        {
            Instances = instances;
        }
    }
}
