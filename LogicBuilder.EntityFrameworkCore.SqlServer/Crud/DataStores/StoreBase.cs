using LogicBuilder.Data;
using LogicBuilder.Expressions.Utils.Strutures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores
{
    abstract public class StoreBase : IStore
    {
        public StoreBase(DbContext context)
        {
            _unitOfWork = new UnitOfWork(context);
        }

        #region Fields
        internal readonly IUnitOfWork _unitOfWork;
        #endregion Fields

        #region Methods
        public async Task<ICollection<T>> GetAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IQueryable<T>> queryFunc = null, ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null, ICollection<FilteredIncludeExpression> filteredIncludes = null) where T : BaseData
        {
            return await _unitOfWork.GetRepository<T>().GetAsync
            (
                filter,
                queryFunc,
                includeProperties,
                filteredIncludes
            );
        }

        public async Task<IQueryable<T>> GetQueryableAsync<T>(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IQueryable<T>> queryableFunc = null) where T : BaseData
        {
            return await _unitOfWork.GetRepository<T>().GetQueryableAsync
            (
                filter,
                queryableFunc
            );
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> filter = null) where T : BaseData
        {
            return await _unitOfWork.GetRepository<T>().CountAsync(filter);
        }

        public async Task<TReturn> QueryAsync<T, TReturn>(Func<IQueryable<T>, TReturn> queryableFunc, ICollection<Func<IQueryable<T>, IIncludableQueryable<T, object>>> includeProperties = null) where T : BaseData
        {
            return await _unitOfWork.GetRepository<T>().QueryAsync(queryableFunc, includeProperties);
        }

        public async Task<bool> SaveAsync<T>(ICollection<T> entities) where T : BaseData
        {
            _unitOfWork.GetMapper<T>().AddChanges(entities);
            return await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> SaveGraphsAsync<T>(ICollection<T> entities) where T : BaseData
        {
            _unitOfWork.GetMapper<T>().AddGraphChanges(entities);
            return await _unitOfWork.SaveChangesAsync();
        }

        public void AddChanges<T>(ICollection<T> entities) where T : BaseData
        {
            _unitOfWork.GetMapper<T>().AddChanges(entities);
        }

        public void AddGraphChanges<T>(ICollection<T> entities) where T : BaseData
        {
            _unitOfWork.GetMapper<T>().AddGraphChanges(entities);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _unitOfWork.SaveChangesAsync();
        }
        #endregion Methods
    }
}
