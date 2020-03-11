using System;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class ConnectionAttribute : Attribute
    {
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
