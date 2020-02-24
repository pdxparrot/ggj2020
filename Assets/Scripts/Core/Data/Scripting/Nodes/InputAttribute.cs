using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class InputAttribute : Attribute
    {
        public string Name { get; private set; }

        public InputAttribute(string name)
        {
            Name = name;
        }
    }
}
