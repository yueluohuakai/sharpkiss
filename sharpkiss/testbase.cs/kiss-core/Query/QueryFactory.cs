using Kiss.Utils;

namespace Kiss.Query
{
    /// <summary>
    /// use this class to create query
    /// </summary>
    internal class QueryFactory
    {
        /// <summary>
        /// create a query 
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static IQuery Create(string provider)
        {
            if (StringUtil.IsNullOrEmpty(provider))
                throw new QueryException("ConnectionString provider name is not set.");

            return ServiceLocator.Instance.Resolve(provider) as IQuery;
        }
    }
}
