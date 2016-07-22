using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using Kiss.Utils;

namespace Kiss
{
    /// <summary>
    /// 扩展属性的存储结构
    /// </summary>
    [Serializable]
    public class ExtendedAttributes : JCopy
    {
        #region fields / properties

        private NameValueCollection _extendedAttributes = new NameValueCollection();

        public int ExtendedAttributesCount
        {
            get { return _extendedAttributes.Count; }
        }

        /// <summary>
        /// extend attribute keys
        /// </summary>
        public NameValueCollection.KeysCollection Keys
        {
            get
            {
                return _extendedAttributes.Keys;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual string this[string key]
        {
            get { return GetExtendedAttribute(key); }
            set { SetExtendedAttribute(key, value); }
        }

        #endregion

        #region Get / Set

        public string GetExtendedAttribute(string name)
        {
            return _extendedAttributes[name];
        }

        public void SetExtendedAttribute(string name, string value)
        {
            if (value == null)
                _extendedAttributes.Remove(name);
            else
                _extendedAttributes[name] = value;
        }

        #endregion

        #region GetValue

        public object GetValue<T>(string name, T defaultValue)
        {
            string value = GetExtendedAttribute(name);

            if (StringUtil.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            string type = typeof(T).Name;

            switch (type)
            {
                case "Boolean":
                    bool b;
                    if (bool.TryParse(value, out b))
                    {
                        return b;
                    }
                    return defaultValue;
                case "Int32":
                    int i;
                    if (Int32.TryParse(value, out i))
                        return i;
                    return defaultValue;
                case "DateTime":
                    DateTime dt;
                    if (DateTime.TryParse(value, out dt))
                        return dt;
                    return defaultValue;
                case "String":
                    if (!StringUtil.IsNullOrEmpty(value))
                        return value;
                    return defaultValue;
                default:
                    throw new NotSupportedException();
            }
        }

        public bool HasValue(string name)
        {
            return !StringUtil.IsNullOrEmpty(_extendedAttributes[name]);
        }

        #endregion

        #region override

        public override object Copy()
        {
            ExtendedAttributes ea = this.CreateNewInstance() as ExtendedAttributes;
            ea._extendedAttributes = new NameValueCollection(this._extendedAttributes);
            return ea;
        }

        #endregion

        #region Serialization

        public SerializerData GetSerializerData()
        {
            SerializerData data = new SerializerData();

            string keys = null;
            string values = null;

            Serializer.ConvertFromNameValueCollection(this._extendedAttributes, ref keys, ref values);
            data.Keys = keys;
            data.Values = values;

            return data;
        }

        public void SetSerializerData(SerializerData data)
        {
            this._extendedAttributes = Serializer.ConvertToNameValueCollection(data.Keys, data.Values);

            if (this._extendedAttributes == null)
                _extendedAttributes = new NameValueCollection();
        }

        public void SetData(string keys, string values)
        {
            SerializerData d = new SerializerData();
            d.Keys = keys;
            d.Values = values;

            SetSerializerData(d);
        }

        public void GetData(out string keys, out string values)
        {
            SerializerData d = GetSerializerData();
            keys = d.Keys;
            values = d.Values;
        }

        public void SaveExtProp(NameValueCollection data)
        {
            foreach (string key in data.Keys)
            {
                SetExtendedAttribute(key, data[key]);
            }
        }

        #endregion
    }

    /// <summary>
    /// 扩展字段的存储数据接口
    /// </summary>
    public struct SerializerData
    {
        public string Keys;
        public string Values;
    }

    [Serializable]
    public abstract class JCopy
    {
        private static readonly Hashtable _objects = new Hashtable();

        protected object CreateNewInstance()
        {
            ConstructorInfo ci = _objects[this.GetType()] as ConstructorInfo;
            if (ci == null)
            {
                ci = this.GetType().GetConstructor(new Type[0]);
                _objects[this.GetType()] = ci;
            }

            return ci.Invoke(null);
        }

        public virtual object Copy()
        {
            return CreateNewInstance();
        }
    }
}
