using System;
using Kiss.Security;

namespace Kiss.Plugin
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class PluginAttribute : Attribute, IPlugin
    {
        #region IPlugin Members

        public string Name { get; set; }

        public Type Decorates { get; set; }

        public int SortOrder { get; set; }

        public virtual bool IsAuthorized(Principal user)
        {
            return true;
        }

        public virtual bool IsEnabled
        {
            get { return true; }
        }

        #endregion

        public bool Equals(IPlugin other)
        {
            return Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase)
                && Decorates.Equals(other.Decorates)
                && IsEnabled.Equals(other.IsEnabled);
        }
    }
}
