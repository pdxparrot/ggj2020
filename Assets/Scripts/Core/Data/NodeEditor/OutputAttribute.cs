using System;

namespace pdxpartyparrot.Core.Data.NodeEditor
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OutputAttribute : Attribute
    {

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public OutputAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
