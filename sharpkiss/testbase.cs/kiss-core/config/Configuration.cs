using Kiss.Utils;
using Kiss.XmlTransform;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web;
using System.Xml;

namespace Kiss.Config
{
    /// <summary>
    /// 自定义配置
    /// </summary>
    internal class Configuration
    {
        #region fields

        private XmlNode root;

        private XmlNode _EmptyNode;

        private static readonly object _synclocker = new object();

        #endregion

        #region ctor

        private Configuration()
        {
        }

        #endregion

        #region methods

        public static Configuration GetConfig()
        {
            Configuration config = Singleton<Configuration>.Instance;

            if (config == null)
            {
                lock (_synclocker)
                {
                    config = Singleton<Configuration>.Instance;

                    if (config == null)
                    {
                        config = new Configuration();
                        config.Init();

                        Singleton<Configuration>.Instance = config;
                    }
                }
            }

            return config;
        }

        private void Init()
        {
            // 合并kiss.local.config和kiss.config
            string rootpath = AppDomain.CurrentDomain.BaseDirectory;

            if (HttpContext.Current != null)
                rootpath = Path.Combine(rootpath, "App_Data");

            if (File.Exists(Path.Combine(rootpath, "kiss.config")))
            {
                XmlDocument x = new XmlDocument();
                x.Load(Path.Combine(rootpath, "kiss.config"));

                string localfile = FileUtil.FormatDirectory(XmlUtil.GetStringAttribute(x.DocumentElement, "local", HttpContext.Current == null ? ".kiss.local.config" : Path.Combine(".App_Data", "kiss.local.config")));

                if (File.Exists(localfile))
                {
                    x.Load(localfile);
                }

                root = x.DocumentElement;

                _EmptyNode = x.CreateElement("__empty__");

            }
            else
            {
                XmlDocument xml = new XmlDocument();

                root = xml;

                _EmptyNode = xml.CreateElement("__empty__");
            }

            // 合并数据库连接字符串
            XmlNode node_conn = root.SelectSingleNode("connectionStrings");

            if (node_conn != null && node_conn.ChildNodes.Count > 0)
            {
                FieldInfo fi = typeof(ConfigurationElementCollection)
                     .GetField("bReadOnly", BindingFlags.Instance | BindingFlags.NonPublic);

                if (fi != null)
                {
                    fi.SetValue(ConfigurationManager.ConnectionStrings, false);

                    foreach (XmlNode item in node_conn.ChildNodes)
                    {
                        if (item.Name == "clear")
                        {
                            ConfigurationManager.ConnectionStrings.Clear();
                        }
                        else if (item.Name == "add")
                        {
                            string name = XmlUtil.GetStringAttribute(item, "name", null);
                            if (string.IsNullOrEmpty(name)) continue;

                            string conn = XmlUtil.GetStringAttribute(item, "connectionString", null);
                            if (string.IsNullOrEmpty(name)) continue;

                            string provider = XmlUtil.GetStringAttribute(item, "providerName", null);
                            if (string.IsNullOrEmpty(provider)) continue;

                            if (ConfigurationManager.ConnectionStrings[name] != null)
                                ConfigurationManager.ConnectionStrings.Remove(name);

                            // hack sqlite connection string
                            if (provider == "System.Data.Sqlite")
                            {
                                NameValueCollection nv = StringUtil.DelimitedEquation2NVCollection(";", conn);
                                string ds = nv["Data Source"];
                                if (!string.IsNullOrWhiteSpace(ds))
                                {
                                    ds = ds.Trim();

                                    nv["Data Source"] = FileUtil.FormatDirectory(ds);

                                    conn = StringUtil.ToQueryString(nv, ";");
                                }
                            }

                            ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings(name, conn, provider));
                        }
                    }

                    fi.SetValue(ConfigurationManager.ConnectionStrings, true);
                }
                else
                {
                    LogManager.GetLogger<Configuration>().Warn("Can't modify ConfigurationManager.ConnectionStrings.");
                }
            }
        }

        public XmlNode GetSection(string nodePath)
        {
            return root.SelectSingleNode(nodePath) ?? _EmptyNode;
        }

        #endregion
    }
}





