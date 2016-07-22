using System;
using System.Collections.Generic;
using System.Xml;

namespace Kiss.Plugin
{
    public class PluginSetting : ExtendedAttributes
    {
        internal PluginSetting()
        {
        }

        public virtual string Name { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Enable { get; set; }
        public virtual XmlNode Node { get; set; }
    }

    public class PluginSettingDecorator : PluginSetting
    {
        private PluginSetting _inner;

        public override string Name
        {
            get
            {
                return _inner.Name;
            }
            set
            {
                _inner.Name = value;
            }
        }

        public override string Description
        {
            get
            {
                return _inner.Description;
            }
            set
            {
                _inner.Description = value;
            }
        }

        public override bool Enable
        {
            get
            {
                return _inner.Enable;
            }
            set
            {
                _inner.Enable = value;
            }
        }

        public override string Title
        {
            get
            {
                return _inner.Title;
            }
            set
            {
                _inner.Title = value;
            }
        }

        public override XmlNode Node
        {
            get
            {
                return _inner.Node;
            }
            set
            {
                _inner.Node = value;
            }
        }

        public override string this[string key]
        {
            get
            {
                return _inner[key];
            }
            set
            {
                _inner[key] = value;
            }
        }

        public PluginSettingDecorator(PluginSetting setting)
        {
            _inner = setting;
        }
    }

    /// <summary>
    /// settings of plugin
    /// </summary>
    public class PluginSettings : List<PluginSetting>
    {
        internal PluginSettings()
        {
        }

        internal PluginSetting FindByName(string name)
        {
            PluginSetting setting = Find(delegate(PluginSetting s) { return string.Equals(s.Name, name, StringComparison.InvariantCultureIgnoreCase); });
            if (setting == null)
                setting = new PluginSetting()
                {
                    Name = name,
                    Enable = true
                };

            return setting;
        }

        /// <summary>
        /// Find settings of the plugin
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PluginSetting Get<T>() where T : IPluginInitializer
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// get plugin settings of specified plugin name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PluginSetting GetByPluginName(string name)
        {
            return PluginConfig.Instance.Settings.FindByName(name);
        }

        static PluginSetting Get(Type type)
        {
            PluginBootstrapper boot = PluginBootstrapper.Instance;

            if (boot._pluginSettings.ContainsKey(type.Name))
                return boot._pluginSettings[type.Name];

            return null;
        }
    }
}
