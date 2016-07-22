using Kiss.Config;
using Kiss.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Text;

namespace Kiss.Query
{
    /// <summary>
    /// 查询条件
    /// </summary>
    public class QueryCondition : ICloneable
    {
        #region ctor

        public QueryCondition()
        {
        }

        public QueryCondition(string connstr_name)
        {
            SetConnectionStringName(connstr_name);
        }

        #endregion

        #region props

        private Dictionary<string, object> _datas = new Dictionary<string, object>();

        public virtual object this[string key]
        {
            get
            {
                if (_datas.ContainsKey(key))
                    return _datas[key];

                return null;
            }
            set
            {
                _datas[key] = value;
            }
        }

        /// <summary>
        /// query id
        /// </summary>
        public string Id { get; set; }

        public int PageIndex { get; set; }

        /// <summary>
        /// 未设置
        /// </summary>
        private int _pageSize = -1;

        public int PageSize { get { return _pageSize; } set { _pageSize = value; } }

        public virtual bool Paging { get { return PageSize > 0; } }

        public int PageCount { get { if (Paging) return (int)Math.Ceiling(TotalCount * 1.0 / PageSize); return 0; } }

        public int PageIndex1 { get { return PageIndex + 1; } }

        public int TotalCount { get; set; }

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        public Dictionary<string, object> Parameters { get { return _parameters; } }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public virtual string ConnectionString
        {
            get
            {
                if (ConnectionStringSettings == null)
                    throw new QueryException("ConnectionStringSettings is not set.");
                return ConnectionStringSettings.ConnectionString;
            }
        }

        /// <summary>
        /// Provider
        /// </summary>
        public virtual string ProviderName
        {
            get
            {
                if (ConnectionStringSettings == null)
                    throw new QueryException("ConnectionStringSettings is not set.");
                return ConnectionStringSettings.ProviderName;
            }
        }

        private ConnectionStringSettings _connectionStringSettings;
        /// <summary>
        /// 连接字符串设置
        /// </summary>
        public virtual ConnectionStringSettings ConnectionStringSettings { get { return _connectionStringSettings; } set { _connectionStringSettings = value; } }

        private string where;
        /// <summary>
        /// where clause
        /// </summary>
        public string WhereClause
        {
            get
            {
                if (where == null)
                {
                    StringBuilder sb = new StringBuilder();
                    AppendWhere(sb);
                    where = sb.ToString();
                }

                return where;
            }
            set { where = value; }
        }

        private List<string> _allowedOrderbyColumns = new List<string>();
        /// <summary>
        /// allowed order by column names
        /// </summary>
        public List<string> AllowedOrderbyColumns { get { return _allowedOrderbyColumns; } }

        /// <summary>
        /// comma delimited allowed order by column name
        /// </summary>
        public string orderbys { get { return StringUtil.CollectionToCommaDelimitedString(AllowedOrderbyColumns); } }

        /// <summary>
        /// order by clause
        /// </summary>
        public string OrderByClause
        {
            get
            {
                List<string> list = new List<string>();
                foreach (var item in _orderbyItems)
                {
                    if (!AllowedOrderbyColumns.Contains(item.Key))
                        continue;
                    list.Add(string.Format("{0} {1}", item.Key, item.Value ? "ASC" : "DESC"));
                }
                return StringUtil.CollectionToCommaDelimitedString(list);
            }
        }

        private string tableName;
        /// <summary>
        /// 表名称
        /// </summary>
        public string TableName
        {
            get
            {
                if (tableName == null)
                    tableName = GetTableName();
                return tableName;
            }
            set { tableName = value; }
        }

        private string tableField;
        /// <summary>
        /// 主键名称
        /// </summary>
        public string TableField
        {
            get
            {
                if (tableField == null)
                    tableField = GetTableField();

                return tableField;
            }
            set { tableField = value; }
        }

        /// <summary>
        /// 是否将默认的orderby 放在排序最前
        /// </summary>
        public bool IsAddOrderBy2First { get; set; }

        private List<KeyValuePair<string, bool>> _orderbyItems = new List<KeyValuePair<string, bool>>();

        /// <summary>
        /// add sort column( desc ) and set the column to sortable
        /// </summary>
        /// <param name="column">column name</param>
        public void AddOrderby(string column)
        {
            AddOrderby(column, false);
        }

        /// <summary>
        /// add sort column and set the column to sortable
        /// </summary>
        /// <param name="column">column name</param>
        /// <param name="asc">asc</param>
        public void AddOrderby(string column, bool asc)
        {
            if (!AllowedOrderbyColumns.Contains(column))
                AllowedOrderbyColumns.Add(column);

            AppendOrderby(column, asc);
        }

        public void InsertOrderby(int index, string column)
        {
            InsertOrderby(index, column, false);
        }

        public void InsertOrderby(int index, string column, bool asc)
        {
            if (!AllowedOrderbyColumns.Contains(column))
                AllowedOrderbyColumns.Add(column);

            AppendOrderby(index, column, asc);
        }

        /// <summary>
        /// add sort column
        /// </summary>
        public void AppendOrderby(string column, bool asc)
        {
            if (!_orderbyItems.Exists(delegate(KeyValuePair<string, bool> p) { return string.Equals(p.Key, column, StringComparison.InvariantCultureIgnoreCase); }))
                _orderbyItems.Add(new KeyValuePair<string, bool>(column, asc));
        }

        public void AppendOrderby(int index, string column, bool asc)
        {
            if (!_orderbyItems.Exists(delegate(KeyValuePair<string, bool> p) { return string.Equals(p.Key, column, StringComparison.InvariantCultureIgnoreCase); }))
                _orderbyItems.Insert(index, new KeyValuePair<string, bool>(column, asc));
        }

        #endregion

        /// <summary>
        /// set connection string name
        /// </summary>
        /// <param name="name"></param>
        public void SetConnectionStringName(string name)
        {
            _connectionStringSettings = ConfigBase.GetConnectionStringSettings(name);
        }

        #region event

        public class BeforeQueryEventArgs : EventArgs
        {
            public static readonly new BeforeQueryEventArgs Empty = new BeforeQueryEventArgs();

            public string Method { get; set; }

            public string DbProviderName { get; set; }
        }

        /// <summary>
        /// this event is fired before query condition is translated to sql statement.
        /// </summary>
        public static event EventHandler<BeforeQueryEventArgs> BeforeQuery;

        protected virtual void OnBeforeQuery(BeforeQueryEventArgs e)
        {
            EventHandler<BeforeQueryEventArgs> handler = BeforeQuery;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// 是否允许多次抛出事件
        /// </summary>
        public bool EnableFireEventMulti { get; set; }

        public int EventFiredTimes { get; set; }
        public virtual void FireBeforeQueryEvent(string method, string dbProviderName)
        {
            if (EventFiredTimes > 0 && !EnableFireEventMulti) return;

            lock (this)
            {
                if (EventFiredTimes > 0 && !EnableFireEventMulti) return;

                EventFiredTimes++;
            }

            OnBeforeQuery(new BeforeQueryEventArgs() { Method = method, DbProviderName = dbProviderName });
        }

        #endregion

        #region virtual / abstract

        protected virtual string GetTableName() { return null; }
        protected virtual string GetTableField() { return "*"; }

        /// <summary>
        /// load query conditions(normally from querystring)
        /// </summary>
        public virtual void LoadCondidtion()
        {
        }

        protected virtual void AppendWhere(StringBuilder where) { }

        #endregion

        /// <summary>
        /// no paging query
        /// </summary>
        public QueryCondition NoPaging()
        {
            return NoPaging(0);
        }

        /// <summary>
        /// top N query
        /// </summary>
        /// <param name="topN"></param>
        public QueryCondition NoPaging(int topN)
        {
            PageSize = 0;
            TotalCount = topN;

            return this;
        }

        /// <summary>
        /// 获取记录Id列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> GetRelationIds<T>()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            return provider.GetRelationIds<T>(this);
        }

        public List<int> GetRelationIds()
        {
            return GetRelationIds<int>();
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <returns></returns>
        public int GetRelationCount()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            return provider.Count(this);
        }

        /// <summary>
        /// get IDataReader from querycondition, no cache
        /// </summary>
        /// <returns></returns>
        public IDataReader GetReader()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            return provider.GetReader(this);
        }

        /// <summary>
        /// get DataTable from query condition
        /// </summary>
        /// <returns></returns>
        public DataTable GetDataTable()
        {
            IQuery provider = QueryFactory.Create(ProviderName);

            return provider.GetDataTable(this);
        }

        public Hashtable GetHashtable()
        {
            using (IDataReader rdr = GetReader())
            {
                DataTable schema = rdr.GetSchemaTable();
                if (rdr.Read())
                {
                    return DataUtil.GetHashtable(rdr, schema);
                }

                return null;
            }
        }

        public object Clone()
        {
            return (QueryCondition)MemberwiseClone();
        }
    }
}
