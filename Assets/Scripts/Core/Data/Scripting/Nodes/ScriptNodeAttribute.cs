using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ScriptNodeAttribute : Attribute
    {
        public enum AllowedInstances
        {
            Single,
            Multiple,
        }

        public string Name { get; private set; }

        public AllowedInstances Instances { get; private set; }

        public bool AllowMultiple => AllowedInstances.Multiple == Instances;

        public ScriptNodeAttribute(string name)
        {
            Name = name;
            Instances = AllowedInstances.Multiple;
        }

        public ScriptNodeAttribute(string name, AllowedInstances instances)
        {
            Name = name;
            Instances = instances;
        }
    }
}
