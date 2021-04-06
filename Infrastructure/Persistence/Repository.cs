using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Application.Common.Interfaces;

namespace Infrastructure.Persistence
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _context.Set<T>().AddRange(entities);
        }

        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _context.Set<T>().Where(expression);
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(TKey key)
        {
            return _context.Set<T>().Find(key);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}