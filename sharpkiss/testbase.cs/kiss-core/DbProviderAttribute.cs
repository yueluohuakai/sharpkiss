using System;
using Kiss.Plugin;

namespace Kiss
{
    /// <summary>
    /// use this attribute to mark a database provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DbProviderAttribute : PluginAttribute
    {
        /// <summary>
        /// provider name. etc:System.Data.SQLite
        /// </summary>
        public string ProviderName { get; set; }
    }
}
