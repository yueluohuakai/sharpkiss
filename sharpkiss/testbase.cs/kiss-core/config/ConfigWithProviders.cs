using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;

namespace Kiss.Config
{
    /// <summary>
    /// 带Provider的配置
    /// </summary>
    public abstract class ConfigWithProviders : ConfigBase
    {
        #region props

        internal string ConfigName { get; private set; }

        /// <summary>
        /// Provider列表
        /// </summary>
        [ConfigProp ( "providers", ConfigPropAttribute.DataType.Unknown )]
        public Dictionary<string, Provider> Providers { get; protected set; }

        /// <summary>
        /// 默认的Provider
        /// </summary>
        [ConfigProp ( "defaultProvider", ConfigPropAttribute.DataType.String )]
        public string DefaultProvider { get; protected set; }

        #endregion

        #region override

        protected override void LoadValuesFromConfigurationXml ( XmlNode node )
        {
            base.LoadValuesFromConfigurationXml ( node );

            ConfigName = node.Name;

            Providers = new Dictionary<string, Provider> ( );

            foreach ( XmlNode child in node.ChildNodes )
            {
                if ( child.Name == "providers" )
                    GetProviders ( child );
            }

            LoadConfigsFromConfigurationXml ( node );
        }

        #endregion

        #region virtual

        /// <summary>
        /// 需要重写的方法，用来读取其他节点
        /// </summary>
        protected virtual void LoadConfigsFromConfigurationXml ( XmlNode node )
        { }

        #endregion

        #region methods

        /// <summary>
        /// 加载Provider
        /// </summary>
        protected void GetProviders ( XmlNode node )
        {
            foreach ( XmlNode provider in node.ChildNodes )
            {
                switch ( provider.Name )
                {
                    case "add":
                        Providers.Add ( provider.Attributes[ "name" ].Value, new Provider ( provider.Attributes ) );
                        break;
                    case "remove":
                        Providers.Remove ( provider.Attributes[ "name" ].Value );
                        break;
                    case "clear":
                        Providers.Clear ( );
                        break;
                }
            }
        }

        #endregion

        /// <summary>
        /// Provider实体类
        /// </summary>
        public class Provider
        {
            #region props

            public string Name { get; private set; }

            public string Type { get; private set; }

            public NameValueCollection Attributes { get; private set; }

            #endregion

            #region ctor

            public Provider ( XmlAttributeCollection attributes )
            {
                Name = attributes[ "name" ].Value;
                Type = attributes[ "type" ].Value;
                Attributes = new NameValueCollection ( );

                foreach ( XmlAttribute att in attributes )
                {
                    if ( att.Name != "name"
                        && att.Name != "type"
                        && Attributes.Get ( att.Name ) == null )
                    {
                        Attributes.Add ( att.Name, att.Value );
                    }
                }
            }

            #endregion
        }
    }
}
