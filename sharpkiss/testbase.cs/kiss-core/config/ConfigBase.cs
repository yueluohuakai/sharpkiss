using Kiss.Utils;
using System;
using System.Configuration;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace Kiss.Config
{
    /// <summary>
    /// base config class
    /// </summary>
    [Serializable]
    public class ConfigBase
    {
        /// <summary>
        /// default connection string settings
        /// </summary>
        public static ConnectionStringSettings DefaultConnectionStringSettings { get; set; }

        /// <summary>
        /// read config from xml node
        /// </summary>
        /// <param name="node"></param>
        protected virtual void LoadValuesFromConfigurationXml(XmlNode node)
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                object[] arg = info.GetCustomAttributes(typeof(ConfigPropAttribute), true);
                if (arg == null || arg.Length == 0) continue;

                ConfigPropAttribute prop = arg[0] as ConfigPropAttribute;
                if (prop.Type == ConfigPropAttribute.DataType.Unknown)
                    continue;

                bool hasDefaultValue = prop.DefaultValue != null;
                object value = null;
                switch (prop.Type)
                {
                    case ConfigPropAttribute.DataType.Int:
                        hasDefaultValue = hasDefaultValue && prop.DefaultValue is Int32;
                        value = XmlUtil.GetIntAttribute(node, prop.Name, hasDefaultValue ? (int)prop.DefaultValue : 0);
                        break;
                    case ConfigPropAttribute.DataType.Boolean:
                        hasDefaultValue = hasDefaultValue && prop.DefaultValue is Boolean;
                        value = XmlUtil.GetBoolAttribute(node, prop.Name, hasDefaultValue ? (Boolean)prop.DefaultValue : false);
                        break;
                    case ConfigPropAttribute.DataType.String:
                        hasDefaultValue = hasDefaultValue && prop.DefaultValue is string;
                        value = XmlUtil.GetStringAttribute(node, prop.Name, hasDefaultValue ? (string)prop.DefaultValue : string.Empty);
                        break;
                    case ConfigPropAttribute.DataType.Strings:
                        hasDefaultValue = hasDefaultValue && prop.DefaultValue is string;
                        value = StringUtil.Split(XmlUtil.GetStringAttribute(node, prop.Name, hasDefaultValue ? (string)prop.DefaultValue : string.Empty),
                            ",",
                            true,
                            false);
                        break;
                    case ConfigPropAttribute.DataType.Long:
                        hasDefaultValue = hasDefaultValue && prop.DefaultValue is long;
                        value = XmlUtil.GetLongAttribute(node, prop.Name, hasDefaultValue ? (long)prop.DefaultValue : 0L);
                        break;
                    default:
                        break;
                }

                if (info.CanWrite && value != null)
                    info.SetValue(this, value, null);
            }

            foreach (XmlAttribute attr in node.Attributes)
            {
                _ext.SetExtendedAttribute(attr.Name, attr.Value);
            }
        }

        #region GetConfig

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>调用此方法的配置类需要添加ConfigNode标签</remarks>
        public static T GetConfig<T>() where T : ConfigBase
        {
            return GetConfig(typeof(T)) as T;
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <returns></returns>
        /// <remarks>调用此方法的配置类需要添加ConfigNode标签, 程序集需要添加Config标签</remarks>
        public static ConfigBase GetConfig(Assembly assembly)
        {
            object[] arg = assembly.GetCustomAttributes(typeof(ConfigAttribute), false);
            if (arg == null || arg.Length == 0)
                throw new ConfigException(string.Format(Resource.NoConfigInAssembly, assembly.FullName));

            ConfigAttribute attr = arg[0] as ConfigAttribute;

            Type t = attr.ConfigType;
            if (t == null)
                throw new ConfigException(string.Format(Resource.ConfigNoType, assembly.FullName));

            return GetConfig(t);
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <returns></returns>
        /// <remarks>调用此方法的配置类需要添加ConfigNode标签</remarks>
        public static ConfigBase GetConfig(Type type)
        {
            object[] arg = type.GetCustomAttributes(typeof(ConfigNodeAttribute), true);
            if (arg == null || arg.Length == 0)
                throw new ConfigException(string.Format(Resource.NoConfigNodeAttribute, type.Name));

            ConfigNodeAttribute node = arg[0] as ConfigNodeAttribute;

            return GetConfig(type, node.Name, node.UseCache);
        }

        public static ConfigBase GetConfig(string sectionName)
        {
            Configuration config = Configuration.GetConfig();

            string key = FormatCacheKey(sectionName);

            ConfigBase typeConfig = HttpRuntime.Cache.Get(key) as ConfigBase;

            if (typeConfig == null)
            {
                XmlNode node = config.GetSection(sectionName);

                typeConfig = new ConfigBase();
                typeConfig.LoadValuesFromConfigurationXml(node);

                HttpRuntime.Cache.Insert(key,
                    typeConfig, null, DateTime.MaxValue, Cache.NoSlidingExpiration);
            }

            return typeConfig;
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sectionName">节点名称</param>
        /// <param name="useCache">是否缓存配置实例</param>
        /// <returns></returns>
        protected static T GetConfig<T>(string sectionName, bool useCache) where T : ConfigBase
        {
            Type thisType = typeof(T);
            return GetConfig(thisType, sectionName, useCache) as T;
        }

        /// <summary>
        /// 获取配置实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="section"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        protected static T GetConfig<T>(XmlNode section, bool useCache) where T : ConfigBase
        {
            Type thisType = typeof(T);
            return GetConfig(thisType, section, useCache) as T;
        }

        private static ConfigBase GetConfig(Type type, string sectionName, bool useCache)
        {
            Configuration config = Configuration.GetConfig();

            string key = FormatCacheKey(type.FullName);

            ConfigBase typeConfig = null;

            if (useCache)
            {
                typeConfig = HttpRuntime.Cache.Get(key) as ConfigBase;
            }

            if (typeConfig == null)
            {
                XmlNode node = config.GetSection(sectionName);

                typeConfig = GetConfig(type, node, useCache);
            }

            return typeConfig;
        }

        private static ConfigBase GetConfig(Type type, XmlNode node, bool useCache)
        {
            ConfigBase config = null;

            string key = FormatCacheKey(type.FullName);

            if (useCache)
            {
                config = HttpRuntime.Cache.Get(key) as ConfigBase;
            }

            if (config == null)
            {
                config = Activator.CreateInstance(type) as ConfigBase;

                if (config != null)
                {
                    config.LoadValuesFromConfigurationXml(node);

                    if (useCache)
                    {
                        HttpRuntime.Cache.Insert(key, 
                            config, 
                            null, 
                            DateTime.MaxValue, Cache.NoSlidingExpiration);
                    }
                }
            }

            return config;
        }

        #endregion

        #region extend props

        private ExtendedAttributes _ext = new ExtendedAttributes();

        public virtual string this[string key]
        {
            get
            {
                return _ext.GetExtendedAttribute(key);
            }
        }

        #endregion

        #region Help

        private static string FormatCacheKey(string key)
        {
            return string.Format("__kiss.config:{0}__", key);
        }

        /// <summary>
        /// get connection string setting 
        /// </summary>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static ConnectionStringSettings GetConnectionStringSettings(string connectionStringName)
        {
            if (StringUtil.IsNullOrEmpty(connectionStringName))
                throw new ConfigException("connectionStringName is not set.");

            ConnectionStringSettings s = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (s == null)
                throw new ConfigException("connectionstring not found! name: " + connectionStringName);

            return s;
        }

        #endregion
    }
}
