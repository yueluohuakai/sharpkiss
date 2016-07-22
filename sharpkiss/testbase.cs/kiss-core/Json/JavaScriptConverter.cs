using System;
using System.Collections.Generic;

namespace Kiss.Json
{
    public abstract class JavaScriptConverter
    {
        // Methods
        protected JavaScriptConverter()
        {
        }

        public abstract object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer);
        public abstract IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer);

        // Properties
        public abstract IEnumerable<Type> SupportedTypes { get; }
    }
}
