using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace StudentProfile.DAL
{
    public class GenericRepository<TEntity,TContext> : IGenericRepository<TEntity, TContext> where TEntity : class where TContext:DbContext
    {
        public TContext Context { get;}

        public TEntity GetById(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }
        public TEntity CreateInstance()
        {
            TEntity newTable = Activator.CreateInstance<TEntity>();
            return newTable;
        }
        public dynamic GetCustomEntity(Type entityName)
        {
            return Context.Set(entityName.GetType());
        }
        public dynamic ExecuteQuery(Type spType, string query, SqlParameter[] paramters)
        {
           return Context.Database.SqlQuery(spType, query, paramters);
        }
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
        }
    }
}
