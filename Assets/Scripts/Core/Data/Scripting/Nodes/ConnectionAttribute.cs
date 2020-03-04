using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ConnectionAttribute : Attribute
    {
        // TODO: rename Direction to match Ports
        public enum Direction
        {
            Input,
            Output
        }

        public string Name { get; private set; }

        public Direction ConnectionDirection { get; private set; }

        public ConnectionAttribute(string name, Direction direction)
        {
            Name = name;
            ConnectionDirection = direction;
        }
    }
}
