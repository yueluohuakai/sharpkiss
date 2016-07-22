using System;

namespace Kiss.Plugin
{
    /// <summary>
    /// Marks a plugin initializer as eligible for auto initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class AutoInitAttribute : Attribute, IPluginDefinition
    {
        private Type initializerType = null;

        public Type InitializerType
        {
            get { return initializerType; }
            set { initializerType = value; }
        }

        public virtual void Init(ServiceLocator sl, ref PluginSetting setting)
        {
            if (InitializerType == null) throw new ArgumentNullException("InitializerType");

            CreateInitializer().Init(sl, ref setting);
        }

        /// <summary>Creates an instance of the initializer defined by this attribute.</summary>
        /// <returns>A new initializer instance.</returns>
        protected virtual IPluginInitializer CreateInitializer()
        {
            return (IPluginInitializer)Activator.CreateInstance(InitializerType);
        }

        public int Priority { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Name { get { return InitializerType.Name; } }
    }
}
