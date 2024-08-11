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
    abstract public class ContextRepositoryBase : IContextRepository
    {
        public ContextRepositoryBase(IStore store, IMapper mapper)
        {
            this._store = store;
            this._mapper = mapper;
        }

        #region Fields
        private readonly IStore _store;
        private readonly IMapper _mapper;
        #endregion Fields

        #region Methods
        public Task<ICollection<TModel>> GetItemsAsync<TModel, TData>(Expression<Func<TModel, bool>> filter = null, Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.GetItemsAsync<TModel, TData>
            (
                _mapper,
                filter,
                queryFunc,
                includeProperties
            );
        }

        public Task<ICollection<TModel>> GetAsync<TModel, TData>(Expression<Func<TModel, bool>> filter = null, Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null, SelectExpandDefinition selectExpandDefinition = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.GetAsync<TModel, TData>
            (
                _mapper,
                filter,
                queryFunc,
                selectExpandDefinition
            );
        }

        public Task<int> CountAsync<TModel, TData>(Expression<Func<TModel, bool>> filter = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.CountAsync<TModel, TData>(_mapper, filter);
        }

        public Task<TModelReturn> QueryAsync<TModel, TData, TModelReturn, TDataReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, SelectExpandDefinition selectExpandDefinition = null,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.QueryAsync<TModel, TData, TModelReturn, TDataReturn>(
                _mapper,
                queryFunc,
                includeProperties,
                selectExpandDefinition);
        }

        public Task<TReturn> QueryAsync<TModel, TData, TModelReturn, TDataReturn, TReturn>(Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc, SelectExpandDefinition selectExpandDefinition = null,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.QueryAsync<TModel, TData, TModelReturn, TDataReturn, TReturn>(
                _mapper,
                queryFunc,
                includeProperties,
                selectExpandDefinition);
        }

        public Task<bool> SaveAsync<TModel, TData>(TModel entity)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.SaveAsync<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public Task<bool> SaveAsync<TModel, TData>(ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.SaveAsync<TModel, TData>(_mapper, entities);
        }

        public Task<bool> SaveGraphAsync<TModel, TData>(TModel entity)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.SaveGraphsAsync<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public Task<bool> SaveGraphsAsync<TModel, TData>(ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.SaveGraphsAsync<TModel, TData>(_mapper, entities);
        }

        public Task<bool> DeleteAsync<TModel, TData>(Expression<Func<TModel, bool>> filter = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return _store.DeleteAsync<TModel, TData>(_mapper, filter);
        }

        public void AddChange<TModel, TData>(TModel entity)
            where TModel : BaseModel
            where TData : BaseData
        {
            _store.AddChanges<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public void AddChanges<TModel, TData>(ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            _store.AddChanges<TModel, TData>(_mapper, entities);
        }

        public void AddGraphChange<TModel, TData>(TModel entity)
            where TModel : BaseModel
            where TData : BaseData
        {
            _store.AddGraphChanges<TModel, TData>(_mapper, new List<TModel> { entity });
        }

        public void AddGraphChanges<TModel, TData>(ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            _store.AddGraphChanges<TModel, TData>(_mapper, entities);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _store.SaveChangesAsync();
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
