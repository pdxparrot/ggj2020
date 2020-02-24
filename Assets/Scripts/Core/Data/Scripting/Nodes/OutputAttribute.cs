using System;

namespace pdxpartyparrot.Core.Data.Scripting.Nodes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class OutputAttribute : Attribute
    {
    }
}
