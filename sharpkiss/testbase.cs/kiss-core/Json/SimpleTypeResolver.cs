using System;

namespace Kiss.Json
{
    public class SimpleTypeResolver : JavaScriptTypeResolver
    {
        // Methods
        public override Type ResolveType(string id)
        {
            return Type.GetType(id);
        }

        public override string ResolveTypeId(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            return type.AssemblyQualifiedName;
        }
    }
}
