using System;
using Kiss.Plugin;

namespace Kiss.Notice
{
    /// <summary>
    /// use this attribute to mark a notice provider
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ChannelAttribute : PluginAttribute
    {
        /// <summary>
        /// provider name. etc:MSG,EMAIL
        /// </summary>
        public string ChannelName { get; set; }

        public string Description { get; set; }
    }
}
