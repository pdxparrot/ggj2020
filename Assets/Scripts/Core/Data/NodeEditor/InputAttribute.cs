using System;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class InputAttribute : Attribute
    {
        public string Name { get; private set; }

        public Type Type { get; private set; }

        public InputAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
