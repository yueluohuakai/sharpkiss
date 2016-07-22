using System.Xml;

namespace Kiss.Utils
{

    /// <summary>
    /// utils of xml operation
    /// </summary>
    public static class XmlUtil
    {

        #region Get***Attribute

        public static string GetStringAttribute(XmlNode node, string key, string defaultValue)
        {
            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
                return node.Attributes[key].Value;
            return defaultValue;
        }

        public static int GetIntAttribute(XmlNode node, string key, int defaultValue)
        {
            int val = defaultValue;

            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
            {
                int.TryParse(node.Attributes[key].Value, out val);
            }
            return val;
        }

        public static bool GetBoolAttribute(XmlNode node, string key, bool defaultValue)
        {
            bool val = defaultValue;

            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
            {
                val = node.Attributes[key].Value.ToBoolean(defaultValue);
            }
            return val;
        }

        public static decimal GetDecimalAttribute(XmlNode node, string key, decimal defaultValue)
        {
            decimal val = defaultValue;
            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
            {
                decimal.TryParse(node.Attributes[key].Value, out val);
            }
            return val;
        }

        public static double GetDoubleAttribute(XmlNode node, string key, double defaultValue)
        {
            double val = defaultValue;
            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
            {
                //StringUtil.ToBoolean
                double.TryParse(node.Attributes[key].Value, out val);
            }
            return val;
        }

        public static long GetLongAttribute(XmlNode node, string key, long defaultValue)
        {
            long val = defaultValue;
            if (node.Attributes[key] != null
                && !string.IsNullOrEmpty(node.Attributes[key].Value))
            {
                long.TryParse(node.Attributes[key].Value, out val);
            }
            return val;
        }

        #endregion

        /// <summary>
        /// xml's IndexOf method
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static int FindElementIndex(XmlNode node)
        {
            int index = 0;
            foreach (XmlNode n in node.ParentNode.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Comment)
                    continue;
                if (n == node)
                    return index;
                index++;
            }

            return -1;
        }
    }
}
