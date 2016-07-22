using System.Collections.Generic;
using System.Data;

namespace Kiss
{
    public interface IWhere
    {
        int Count();
        T Select<T>(string field);
        DataTable Select(params string[] fields);
        List<t> Selects<t>(string field);
        void Delete();
        void Update();
        IWhere Set(string columnandvalue);
        IWhere Set(string column, object value);
        IWhere Where(string where, params object[] args);
        IWhere OrderBy(string column, bool asc);
    }
}
