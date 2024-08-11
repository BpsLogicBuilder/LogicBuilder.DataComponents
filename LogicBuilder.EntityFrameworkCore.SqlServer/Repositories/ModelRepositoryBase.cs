using AutoMapper;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores;
using LogicBuilder.Expressions.Utils.Expansions;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Repositories
{
    abstract public class ModelRepositoryBase<TModel, TData> : IModelRepository<TModel, TData>
        where TModel : BaseModel
            where TData : BaseData
    {
        public ModelRepositoryBase(IStore store, IMapper mapper)
        {
            this._store = store;
            this._mapper = mapper;
        }

        #region Fields
        private readonly IStore _store;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Methods
        public Task<ICollection<TModel>> GetItemsAsync(Expression<Func<TModel, bool>> filter = null, Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
        {
            return _store.GetItemsAsync<TModel, TData>
            (
                _mapper,
                filter,
                queryFunc,
                includeProperties
            );
        }

        public Task<ICollection<TModel>> GetAsync(Expression<Func<TModel, bool>> filter = null, Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null, SelectExpandDefinition selectExpandDefinition = null)
        {
            return _store.GetAsync<TModel, TData>
            (
                _mapper,
                filter,
                queryFunc,
                selectExpandDefinition
            );
        }

        public Task<int> CountAsync(Expression<Func<TModel, bool>> filter = null)
        {
            return _store.CountAsync<TModel, TData>(_mapper, filter);
        }

        public Task<TModelReturn> QueryAsync<TModelReturn, TDataReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
        {
            return _store.QueryAsync<TModel, TData, TModelReturn, TDataReturn>(
                _mapper,
                queryFunc,
                includeProperties);
        }

        public Task<TReturn> QueryAsync<TModelReturn, TDataReturn, TReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
        {
            return _store.QueryAsync<TModel, TData, TModelReturn, TDataReturn, TReturn>(
                _mapper,
                queryFunc,
                includeProperties);
        }

        public Task<bool> SaveAsync(TModel entity)
        {
            return _store.SaveAsync<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public Task<bool> SaveAsync(ICollection<TModel> entities)
        {
            return _store.SaveAsync<TModel, TData>(_mapper, entities);
        }

        public Task<bool> SaveGraphAsync(TModel entity)
        {
            return _store.SaveGraphsAsync<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public Task<bool> SaveGraphsAsync(ICollection<TModel> entities)
        {
            return _store.SaveGraphsAsync<TModel, TData>(_mapper, entities);
        }

        public Task<bool> DeleteAsync(Expression<Func<TModel, bool>> filter = null)
        {
            return _store.DeleteAsync<TModel, TData>(_mapper, filter);
        }

        public void AddChange(TModel entity)
        {
            _store.AddChanges<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public void AddChanges(ICollection<TModel> entities)
        {
            _store.AddChanges<TModel, TData>(_mapper, entities);
        }

        public void AddGraphChange(TModel entity)
        {
            _store.AddGraphChanges<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public void AddGraphChanges(ICollection<TModel> entities)
        {
            _store.AddGraphChanges<TModel, TData>(_mapper, entities);
        }

        public Task<bool> SaveChangesAsync()
        {
            return _store.SaveChangesAsync();
        }

        public void ClearChangeTracker()
        {
            _store.ClearChangeTracker();
        }

        public void DetachAllEntries()
        {
            _store.DetachAllEntries();
        }
        #endregion Methods
    }
}
