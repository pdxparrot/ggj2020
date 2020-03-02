using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConnectionAttribute : Attribute
    {
        public string Name { get; private set; }

        public ConnectionAttribute(string name)
        {
            Name = name;
        }
    }
}
