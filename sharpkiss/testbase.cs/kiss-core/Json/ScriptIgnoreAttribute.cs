using System;

namespace Kiss.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ScriptIgnoreAttribute : Attribute
    {
    }
}
