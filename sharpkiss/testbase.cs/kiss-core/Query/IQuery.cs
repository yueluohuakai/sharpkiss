using System.Collections.Generic;
using System.Data;

namespace Kiss.Query
{
    /// <summary>
    /// 关系查询抽象Provider
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// 获取记录总数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        int Count(QueryCondition condition);

        /// <summary>
        /// 获取记录主键列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<T> GetRelationIds<T>(QueryCondition condition);

        /// <summary>
        /// 获取IDataReader
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        IDataReader GetReader(QueryCondition q);

        DataTable GetDataTable(QueryCondition q);

        IDbTransaction BeginTransaction(string connectionstring, IsolationLevel isolationLevel);
    }
}