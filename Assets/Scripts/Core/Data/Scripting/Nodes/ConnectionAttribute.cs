using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConnectionAttribute : Attribute
    {
        // TODO: rename Direction to match Ports
        public enum ConnectionType
        {
            Input,
            Output
        }

        public string Name { get; private set; }

        public ConnectionType Type { get; private set; }

        public ConnectionAttribute(string name, ConnectionType type)
        {
            Name = name;
            Type = type;
        }
    }
}
