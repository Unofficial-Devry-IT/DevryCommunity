using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Application.Common.Interfaces
{
    public interface IRepository<T, TKey> where T : class
    {
        T GetById(TKey id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void SaveChanges();
    }
}