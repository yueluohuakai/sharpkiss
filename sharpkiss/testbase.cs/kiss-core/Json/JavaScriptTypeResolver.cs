using System;

namespace Kiss.Json
{
    public abstract class JavaScriptTypeResolver
    {
        // Methods
        protected JavaScriptTypeResolver()
        {
        }

        public abstract Type ResolveType(string id);
        public abstract string ResolveTypeId(Type type);
    }
}
