using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Kiss
{
    public interface ILinqContext<T> : IDisposable, IQueryable<T>, IOrderedQueryable<T>, IQueryProvider
    {
        void Add(T item);
        void Add(T item, bool isNew);
        void AddRange(IEnumerable<T> items);
        void AddRange(IEnumerable<T> items, bool inMemorySort);
        void Remove(T value);
        void Remove(IEnumerable<T> items);
        void SubmitChanges();
        void SubmitChanges(bool batch);

        IDbTransaction Transaction { get; set; }
    }
}
