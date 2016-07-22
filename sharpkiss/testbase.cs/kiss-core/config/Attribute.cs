using System;

namespace Kiss.Config
{
    /// <summary>
    /// 标记程序集是否可配置
    /// </summary>
    [AttributeUsage ( AttributeTargets.Assembly, Inherited = false, AllowMultiple = false )]
    public sealed class ConfigAttribute : Attribute
    {
        readonly Type type;

        public ConfigAttribute ( )
        {
        }

        public ConfigAttribute ( Type type )
        {
            this.type = type;
        }
        /// <summary>
        /// 主配置类型
        /// </summary>
        public Type ConfigType { get { return type; } }
    }

    /// <summary>
    /// 标记类是否可配置
    /// </summary>
    [AttributeUsage ( AttributeTargets.Class, Inherited = true, AllowMultiple = false )]
    public sealed class ConfigNodeAttribute : Attribute
    {
        readonly string name;

        public ConfigNodeAttribute ( string name )
        {
            this.name = name;
        }

        /// <summary>
        /// 配置节点名称
        /// </summary>
        public string Name { get { return name; } }

        private bool userCache = true;

        /// <summary>
        /// 是否缓存
        /// </summary>
        public bool UseCache { get { return userCache; } set { userCache = value; } }

        /// <summary>
        /// 描述，用于管理工具
        /// </summary>
        public string Desc { get; set; }
    }

    /// <summary>
    /// 标记属性是否可配置
    /// </summary>
    [AttributeUsage ( AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public sealed class ConfigPropAttribute : Attribute
    {
        readonly string name;
        readonly DataType type;

        public ConfigPropAttribute ( string name )
            : this ( name, DataType.String )
        {
        }

        public ConfigPropAttribute ( string name, DataType type )
        {
            this.name = name;
            this.type = type;
        }

        /// <summary>
        /// 属性key
        /// </summary>
        public string Name { get { return name; } }

        /// <summary>
        /// 属性类型
        /// </summary>
        public DataType Type { get { return type; } }

        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// 描述，用于管理工具
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 可选项，逗号分隔
        /// </summary>
        public string Options { get; set; }

        /// <summary>
        /// 属性值类型
        /// </summary>
        public enum DataType : int
        {
            Int = 0,            
            Boolean = 1,
            String = 2,
            Strings = 3,
            Long = 4,
            Unknown = -1
        }
    }
}
