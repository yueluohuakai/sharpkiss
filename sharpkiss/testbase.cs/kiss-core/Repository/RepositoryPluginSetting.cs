using Kiss.Plugin;
using Kiss.Utils;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace Kiss.Repository
{
	public class RepositoryPluginSetting : PluginSettingDecorator
	{
		public string DefaultConn { get; private set; }
		public string DefaultMasterConn { get; private set; }
		public Dictionary<KeyValuePair<string, string>, ExtendedAttributes> Conns { get; private set; }

		public RepositoryPluginSetting(PluginSetting setting)
			: base(setting)
		{
			// conns
			Conns = new Dictionary<KeyValuePair<string, string>, ExtendedAttributes>();

			if (setting.Node != null)
			{
				XmlNode connsnode = setting.Node.SelectSingleNode("conns");

				if (connsnode != null)
				{
					DefaultConn = XmlUtil.GetStringAttribute(connsnode, "default", string.Empty);
					DefaultMasterConn = XmlUtil.GetStringAttribute(connsnode, "default_master", DefaultConn);

					foreach (XmlNode conn in connsnode.ChildNodes)
					{
						string conn_name = XmlUtil.GetStringAttribute(conn, "conn", string.Empty);
						if (string.IsNullOrEmpty(conn_name)) continue;

						string table = XmlUtil.GetStringAttribute(conn, "table", string.Empty);
						if (string.IsNullOrEmpty(table)) continue;

						ExtendedAttributes attrs = new ExtendedAttributes();
						foreach (XmlAttribute item in conn.Attributes)
						{
							attrs[item.Name] = item.Value;
						}

						Conns[new KeyValuePair<string, string>(conn_name, table)] = attrs;
					}
				}
			}

			// use first connection string if not config
			if (string.IsNullOrEmpty(DefaultConn) && ConfigurationManager.ConnectionStrings.Count > 0)
				DefaultConn = ConfigurationManager.ConnectionStrings[0].Name;
		}
	}
}
