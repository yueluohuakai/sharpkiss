using Kiss.Query;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;

namespace Kiss
{
    public delegate bool ConvertObj<T>(T obj, NameValueCollection param);

    /// <summary>
    /// obj's basic operations
    /// </summary>
    public interface IRepository<T, t> : IRepository<T>
        where T : Obj<t>
    {
        /// <summary>
        /// get single obj by key
        /// </summary>
        T Get(t id);

        T Get(ILinqContext<T> context, t id);

        /// <summary>
        /// get obj list
        /// </summary>
        List<T> Gets(t[] ids);

        List<T> Gets(ILinqContext<T> context, t[] ids);

        T Save(string param, ConvertObj<T> converter);

        T Save(NameValueCollection param, ConvertObj<T> converter);

        List<T> Gets(string commaDelimitedIds);

        List<T> Gets(ILinqContext<T> context, string commaDelimitedIds);

        void DeleteById(params t[] ids);
    }

    public interface IRepository<T> : IRepository
        where T : IQueryObject
    {
        new List<T> Gets(QueryCondition q);

        ILinqContext<T> CreateContext();

        List<T> GetsAll();

        List<T> GetsAll(ILinqContext<T> context);
    }

    public interface IRepository
    {
        object Gets(QueryCondition q);

        DataTable GetDataTable(QueryCondition q);

        int Count(QueryCondition q);

        IWhere Where(string where, params object[] args);

        KeyValuePair<ConnectionStringSettings, ConnectionStringSettings> ConnectionStringSettings { get; set; }
    }
}
