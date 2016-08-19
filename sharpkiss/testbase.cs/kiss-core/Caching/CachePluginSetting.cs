using Kiss.Plugin;
using Kiss.Utils;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Kiss.Caching
{
    public class CachePluginSetting : PluginSettingDecorator
    {
        /// <summary>
        /// cache valid days
        /// </summary>
        public int CacheDay { get; private set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enabled { get; private set; }

        /// <summary>
        /// cache key's namespace
        /// </summary>
        public string Namespace { get; private set; }

        public string DefaultProviderName { get; private set; }

        /// <summary>
        /// cache valid times
        /// </summary>
        public TimeSpan ValidFor { get; private set; }
        public Dictionary<string, string> Configs { get; private set; }

        public CachePluginSetting(PluginSetting setting)
            : base(setting)
        {
            CacheDay = setting["cacheDay"].ToInt(1);
            Enabled = setting.Enable;
            Namespace = setting["namespace"];

            ValidFor = TimeSpan.FromDays(CacheDay);

            // conns
            Configs = new Dictionary<string, string>();

            if (setting.Node != null)
            {
                XmlNode keysnode = setting.Node.SelectSingleNode("keys");

                if (keysnode != null)
                {
                    DefaultProviderName = XmlUtil.GetStringAttribute(keysnode, "default", "nocache");

                    foreach (XmlNode conn in keysnode.ChildNodes)
                    {
                        string name = XmlUtil.GetStringAttribute(conn, "name", string.Empty);
                        if (string.IsNullOrEmpty(name)) continue;

                        string provider = XmlUtil.GetStringAttribute(conn, "provider", string.Empty);
                        if (string.IsNullOrEmpty(provider)) continue;

                        Configs[name] = provider;
                    }
                }
            }
        }
    }
}
