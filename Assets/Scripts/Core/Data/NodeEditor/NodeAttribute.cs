using System;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeAttribute : Attribute
    {
        public string Name { get; private set; }

        public NodeAttribute(string name)
        {
            Name = name;
        }
    }
}
