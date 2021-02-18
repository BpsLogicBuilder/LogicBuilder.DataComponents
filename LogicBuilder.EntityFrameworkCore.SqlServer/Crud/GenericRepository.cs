using LogicBuilder.Data;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud
{
    internal class GenericRepository<T> where T : class
    {
        public GenericRepository(DbContext context)
        {
            this.context = context;
            this.context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.context.ChangeTracker.AutoDetectChangesEnabled = false;
            this.dbSet = context.Set<T>();
        }

        #region Fields
        private readonly DbContext context;
        private readonly DbSet<T> dbSet;
        #endregion Fields

        #region Properties
        #endregion Properties

        #region Methods
        public virtual async Task<ICollection<T>> GetAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> queryableFunc = null,
            ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null,
            ICollection<FilteredIncludeExpression> filteredIncludes = null)
        {
            IQueryable<T> query = this.dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (includeProperties != null)
                query = includeProperties.Aggregate(query, (list, next) => query = next(query));

            return LoadFilteredIncludes(queryableFunc != null ? await queryableFunc(query).ToListAsync() : await query.ToListAsync());

            ICollection<T> LoadFilteredIncludes(List<T> list)
            {
                if (filteredIncludes == null)
                    return list;

                FilteredIncludesHelper.DoExplicitLoading(this.context, list, filteredIncludes);
                return list;
            }

        }

        public virtual async Task<IQueryable<T>> GetQueryableAsync(Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IQueryable<T>> queryableFunc = null)
        {
            IQueryable<T> query = this.dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await Task.Run(() => queryableFunc != null ? queryableFunc(query) : query);
        }

        public virtual async Task<TReturn> QueryAsync<TReturn>(Func<IQueryable<T>, TReturn> queryableFunc, ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null)
        {
            IQueryable<T> query = this.dbSet;

            if (includeProperties != null)
                query = includeProperties.Aggregate(query, (list, next) => query = next(query));

            return await Task.Run(() => queryableFunc(query));
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = this.dbSet;

            if (filter != null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        /// <summary>
        /// Inserts an object graph into the database
        /// </summary>
        /// <param name="t"></param>
        public virtual void InsertGraph(T t)
        {
            this.dbSet.Add(t);
        }

        //private static void Dump(List<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseData>> entries)
        //{
        //    foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseData> entry in entries)
        //        System.Diagnostics.Debug.WriteLine("Type: {0}, State: {1}.", entry.Entity.GetType().Name, entry.State.ToString());
        //}

        /// <summary>
        /// Inserts only the root object - even if there are child objects attached.
        /// </summary>
        /// <param name="t"></param>
        public virtual void Insert(T t)
        {
            this.context.Entry(t).State = EntityState.Added;//Set only the root to Added.
        }

        /// <summary>
        /// Deletes the entity - deafult behavior is Cascade on child objects.  Do we need a DeleteGraph function?
        /// </summary>
        /// <param name="t"></param>
        public virtual void Delete(T t)
        {
            if (this.context.Entry(t).State == EntityState.Detached)
                this.dbSet.Attach(t);

            this.dbSet.Remove(t);
        }

        /// <summary>
        /// Updates the entire graph.  BaseData.EntityState on the root entity must be set to Deleted.
        /// </summary>
        /// <param name="t"></param>
        public virtual void DeleteGraph(T t)
        {
            this.dbSet.Add(t);
            this.context.SetStates(EntityState.Deleted);
        }

        /// <summary>
        /// Updates only the root object - even if there are child objects attached.
        /// </summary>
        /// <param name="t"></param>
        public virtual void Update(T t)
        {
            this.context.DetachMatchingKeyEntries(t);
            this.dbSet.Attach(t);
            this.context.Entry(t).State = EntityState.Modified;
        }


        /// <summary>
        /// Updates the entire graph.  BaseData.EntityState on the root entity must be set to Modified.
        /// BaseData.EntityState on each object remaining determines the action Insert/Modify/Delete
        /// </summary>
        /// <param name="t"></param>
        public virtual void UpdateGraph(T t)
        {
            this.context.DetachMatchingKeyEntries(t);
            this.dbSet.Add(t);
            this.context.ApplyStateChanges();
        }
        #endregion Methods
    }
}
