using Kiss.Query;
using Kiss.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Xml;

namespace Kiss
{
    [Serializable]
    public abstract class QueryObject<T, t> : Obj<t>, IQueryObject
        where T : Obj<t>
    {
        public static T Get(t id)
        {
            return Repository.Get(id);
        }

        public static T Get(ILinqContext<T> context, t id)
        {
            return Repository.Get(context, id);
        }

        /// <summary>
        /// get obj list
        /// </summary>
        public static List<T> Gets(t[] ids)
        {
            return Repository.Gets(ids);
        }

        public static List<T> Gets(ILinqContext<T> context, t[] ids)
        {
            return Repository.Gets(context, ids);
        }

        public static DataTable GetDataTable(QueryCondition q)
        {
            return Repository.GetDataTable(q);
        }

        public static T Save(string param, ConvertObj<T> converter)
        {
            return Repository.Save(param, converter);
        }

        public static T Save(NameValueCollection param, ConvertObj<T> converter)
        {
            return Repository.Save(param, converter);
        }

        public static List<T> Gets(string commaDelimitedIds)
        {
            return Repository.Gets(commaDelimitedIds);
        }

        public static List<T> Gets(ILinqContext<T> context, string commaDelimitedIds)
        {
            return Repository.Gets(context, commaDelimitedIds);
        }

        public static List<T> Gets(QueryCondition q)
        {
            return Repository.Gets(q);
        }

        public static List<T> GetsAll()
        {
            return Repository.GetsAll();
        }

        public static List<T> GetsAll(ILinqContext<T> context)
        {
            return Repository.GetsAll(context);
        }

        public static int Count(QueryCondition q)
        {
            return Repository.Count(q);
        }

        public static IWhere Where(string where, params object[] args)
        {
            return Repository.Where(where, args);
        }

        public static void DeleteById(params t[] ids)
        {
            Repository.DeleteById(ids);
        }

        public static string GetTableName()
        {
            return QueryObject.GetTableName<T>();
        }

        public static IRepository<T, t> Repository
        {
            get
            {
                return QueryObject.GetRepository<T, t>();
            }
        }

        public static ILinqContext<T> CreateContext() { return Repository.CreateContext(); }

        public static KeyValuePair<ConnectionStringSettings, ConnectionStringSettings> ConnectionStringSettings { get { return Repository.ConnectionStringSettings; } }

        public static List<T> ImportFromXml(XmlDocument xml)
        {
            Type t = typeof(T);

            List<PropertyInfo> props = new List<PropertyInfo>(t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly));

            List<T> list = new List<T>();

            XmlNode node = xml.DocumentElement.SelectSingleNode("//" + t.FullName);

            if (node == null) return list;

            foreach (XmlNode n in node.ChildNodes)
            {
                T obj = Activator.CreateInstance<T>();

                foreach (XmlAttribute attr in n.Attributes)
                {
                    PropertyInfo pi = props.Find(p =>
                    {
                        return p.Name.Equals(attr.Name, StringComparison.InvariantCultureIgnoreCase);
                    });

                    if (pi == null || !pi.CanWrite) continue;

                    pi.SetValue(obj, TypeConvertUtil.ConvertTo(attr.Value, pi.PropertyType), null);
                }

                list.Add(obj);
            }

            ILinqContext<T> context = CreateContext();

            // remove old items
            //            
            List<t> ids = new List<t>();

            foreach (var item in list)
            {
                ids.Add(item.Id);
            }

            List<T> old_list = Gets(context, StringUtil.CollectionToCommaDelimitedString(ids));

            foreach (var item in old_list)
            {
                if (list.Find((T obj) => { return item.Id.Equals(obj.Id); }) != null)
                {
                    context.Remove(item);
                }
            }

            // add new items
            //
            foreach (var item in list)
            {
                context.Add(item, true);
            }

            context.SubmitChanges(true);

            return list;
        }

        /// <summary>
        /// export current type's all data to xml
        /// </summary>
        /// <param name="writer"></param>
        /// <example>
        /// <Kiss.Components.ContentModel.Model,Kiss.Components.ContentModel>
        /// <item Id="1" TypeName="" Key="" Category="" Title="产品" Description="" Status="1" DateCreate="0001/1/1 0:00:00" PropertyName="" PropertyValue="" />
        /// </Kiss.Components.ContentModel.Model>
        /// </example>
        public static void ExportToXml(XmlTextWriter writer)
        {
            ExportToXml(writer, GetsAll());
        }

        public static void ExportToXml(XmlTextWriter writer, List<T> datasource)
        {
            Type t = typeof(T);

            writer.WriteStartElement(t.FullName);

            PropertyInfo[] props = t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

            foreach (var item in datasource)
            {
                writer.WriteStartElement("item");

                foreach (var prop in props)
                {
                    if (prop.GetCustomAttributes(typeof(IgnoreAttribute), true).Length > 0)
                        continue;

                    object val = prop.GetValue(item, null);
                    if (val == null) continue;
                    writer.WriteAttributeString(prop.Name, val.ToString());
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }

    [Serializable]
    public abstract class QueryObject<T> : QueryObject, IQueryObject where T : IQueryObject
    {
        public static List<T> Gets(QueryCondition q)
        {
            return Repository.Gets(q);
        }

        public static DataTable GetDataTable(QueryCondition q)
        {
            return Repository.GetDataTable(q);
        }

        public static List<T> GetsAll()
        {
            return Repository.GetsAll();
        }

        public static List<T> GetsAll(ILinqContext<T> context)
        {
            return Repository.GetsAll(context);
        }

        public static int Count(QueryCondition q)
        {
            return Repository.Count(q);
        }

        public static IWhere Where(string where, params object[] args)
        {
            return Repository.Where(where, args);
        }

        public static string GetTableName()
        {
            return QueryObject.GetTableName<T>();
        }

        public static IRepository<T> Repository
        {
            get
            {
                return QueryObject.GetRepository<T>();
            }
        }

        public static ILinqContext<T> CreateContext() { return Repository.CreateContext(); }

        public static KeyValuePair<ConnectionStringSettings, ConnectionStringSettings> ConnectionStringSettings { get { return Repository.ConnectionStringSettings; } }
    }

    [Serializable]
    public abstract class QueryObject : IQueryObject
    {
        internal static IRepository<T, t> GetRepository<T, t>() where T : Obj<t>
        {
            return ServiceLocator.Instance.Resolve<IRepository<T, t>>();
        }

        internal static IRepository<T> GetRepository<T>() where T : IQueryObject
        {
            return ServiceLocator.Instance.Resolve<IRepository<T>>();
        }

        public static IRepository GetRepository(Type type)
        {
            if (type.GetInterface("IQueryObject") == null)
                throw new KissException("type {0} is not inherite from Kiss.IQueryObject", type.FullName);

            Type t2 = typeof(IRepository<>).MakeGenericType(type);

            return ServiceLocator.Instance.Resolve(t2) as IRepository;
        }

        #region events

        /// <summary>
        /// Occurs when the obj is Saved
        /// </summary>
        public static event EventHandler<SavedEventArgs> Saved;

        /// <summary>
        /// Occurs when the class is Saving
        /// </summary>
        public static event EventHandler<SavingEventArgs> Saving;

        /// <summary>
        /// Raises the Saved event.
        /// </summary>
        public static void OnSaved(IQueryObject obj, SaveAction action)
        {
            if (Saved != null)
            {
                Saved(obj, new SavedEventArgs(action));
            }
        }

        /// <summary>
        /// Raises the Saving event
        /// </summary>
        public static void OnSaving(IQueryObject obj, SavingEventArgs e)
        {
            if (Saving != null)
            {
                Saving(obj, e);
            }
        }

        /// <summary>
        /// Raise the batch add/update/create event
        /// </summary>
        public static event EventHandler<BatchEventArgs> Batch;

        public static void OnBatch(Type type)
        {
            EventHandler<BatchEventArgs> handler = Batch;

            if (handler != null)
            {
                handler(null, new BatchEventArgs() { Type = type });
            }
        }

        #endregion

        private string tableName;

        /// <summary>
        /// 对象对应表名称
        /// </summary>
        [Ignore]
        public string TableName
        {
            get
            {
                if (StringUtil.IsNullOrEmpty(tableName))
                    tableName = GetTableName(GetType());

                return tableName;
            }
        }

        /// <summary>
        /// get table name of object type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            object[] attr = type.GetCustomAttributes(typeof(OriginalNameAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                OriginalNameAttribute originalEntityNameAtt = attr[0] as OriginalNameAttribute;
                return originalEntityNameAtt.Name;
            }

            return type.Name;
        }

        internal static string GetTableName<T>() where T : IQueryObject
        {
            return GetTableName(typeof(T));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SavedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavedEventArgs"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        public SavedEventArgs(SaveAction action)
        {
            Action = action;
        }

        private SaveAction _Action;
        /// <summary>
        /// Gets or sets the action that occured when the object was saved.
        /// </summary>
        public SaveAction Action
        {
            get { return _Action; }
            set { _Action = value; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class SavingEventArgs : SavedEventArgs
    {
        /// <summary>
        /// if or not cancel saving
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public SavingEventArgs()
            : base(SaveAction.None)
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="action"></param>
        public SavingEventArgs(SaveAction action)
            : base(action)
        {
            Cancel = false;
        }
    }

    public class BatchEventArgs : EventArgs
    {
        public static readonly new BatchEventArgs Empty = new BatchEventArgs();
        //public string TableName { get; set; }
        public Type Type { get; set; }
    }

    /// <summary>
    /// The action performed by the save event.
    /// </summary>
    public enum SaveAction
    {
        /// <summary>
        /// Default. Nothing happened.
        /// </summary>
        None,
        /// <summary>
        /// It's a new object that has been inserted.
        /// </summary>
        Insert,
        /// <summary>
        /// It's an old object that has been updated.
        /// </summary>
        Update,
        /// <summary>
        /// The object was deleted.
        /// </summary>
        Delete
    }
}
