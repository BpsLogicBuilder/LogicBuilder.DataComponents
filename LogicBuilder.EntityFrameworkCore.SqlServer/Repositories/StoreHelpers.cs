using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.EntityFrameworkCore.SqlServer.Repositories
{
    internal static class StoreHelpers
    {
        internal static async Task<ICollection<TModel>> GetItemsAsync<TModel, TData>(this IStore store, IMapper mapper,
            Expression<Func<TModel, bool>> filter = null,
            Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            //Map the expressions
            Expression<Func<TData, bool>> f = mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
            Expression<Func<IQueryable<TData>, IQueryable<TData>>> mappedQueryFunc = mapper.MapExpression<Expression<Func<IQueryable<TData>, IQueryable<TData>>>>(queryFunc);
            ICollection<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>> includes = mapper.MapIncludesList<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>>(includeProperties);

            //Call the store
            ICollection<TData> list = await store.GetAsync(f,
                mappedQueryFunc?.Compile(),
                includes?.Select(i => i.Compile()).ToList());

            //Map and return the data
            return mapper.Map<IEnumerable<TData>, IEnumerable<TModel>>(list).ToList();
        }

        internal static async Task<TModelReturn> QueryAsync<TModel, TData, TModelReturn, TDataReturn>(this IStore store, IMapper mapper,
            Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc = null,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            //Map the expressions
            Expression<Func<IQueryable<TData>, TDataReturn>> mappedQueryFunc = mapper.MapExpression<Expression<Func<IQueryable<TData>, TDataReturn>>>(queryFunc);
            ICollection<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>> includes = mapper.MapIncludesList<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>>(includeProperties);

            //Call the store
            TDataReturn result = await store.QueryAsync(mappedQueryFunc?.Compile(),
                includes?.Select(i => i.Compile()).ToList());

            return typeof(TModelReturn) == typeof(TDataReturn) ? (TModelReturn)(object)result : mapper.Map<TDataReturn, TModelReturn>(result);
        }

        internal static async Task<TReturn> QueryAsync<TModel, TData, TModelReturn, TDataReturn, TReturn>(this IStore store, IMapper mapper,
            Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc = null,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            //Map the expressions
            Expression<Func<IQueryable<TData>, TDataReturn>> mappedQueryFunc = mapper.MapExpression<Expression<Func<IQueryable<TData>, TDataReturn>>>(queryFunc);
            ICollection<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>> includes = mapper.MapIncludesList<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>>(includeProperties);

            //Call the store
            TDataReturn result = await store.QueryAsync(mappedQueryFunc?.Compile(),
                includes?.Select(i => i.Compile()).ToList());

            return typeof(TReturn) == typeof(TDataReturn) ? (TReturn)(object)result : mapper.Map<TDataReturn, TReturn>(result);
        }

        internal static async Task<int> CountAsync<TModel, TData>(this IStore store, IMapper mapper, Expression<Func<TModel, bool>> filter = null, IDictionary<Type, Type> typeMappings = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            Expression<Func<TData, bool>> f = mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
            return await store.CountAsync(f);
        }

        /*internal static async Task<ICollection<TModel>> PocoQueryAsync<TModel, TData>(this IStore store, IMapper mapper, Expression<Func<TData, bool>> filter = null, Func<IQueryable<TData>, IQueryable<TData>> queryFunc = null, ICollection<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>> includes = null) where TData : BaseData
        {
            return mapper.Map<IEnumerable<TModel>>(await store.GetAsync<TData>(filter, queryFunc, includes == null ? null : includes.Select(i => i.Compile()).ToList())).ToList();
        }*/

        internal static async Task<bool> SaveGraphsAsync<TModel, TData>(this IStore store, IMapper mapper, ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            //IList<TModel> eList = entities.ToList();
            IList<TData> items = mapper.Map<IEnumerable<TData>>(entities).ToList();
            bool success = await store.SaveGraphsAsync<TData>(items);

            IList<TModel> entityList = entities.ToList();
            for (int i = 0; i < items.Count; i++)
                mapper.Map<TData, TModel>(items[i], entityList[i]);

            return success;

            //Blows up with "Method 'Void Add(T)' declared on type 'System.Collections.Generic.ICollection`1[T]' cannot be called with instance of type 'System.Collections.Generic.IEnumerable`1[T]'"
            //mapper.Map<IEnumerable<TData>, IEnumerable<TModel>>(items, entities).ToList();
        }

        internal static async Task<bool> SaveAsync<TModel, TData>(this IStore store, IMapper mapper, ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            IList<TData> items = mapper.Map<IEnumerable<TData>>(entities).ToList();
            bool success = await store.SaveAsync<TData>(items);

            IList<TModel> entityList = entities.ToList();
            for (int i = 0; i < items.Count; i++)
                mapper.Map<TData, TModel>(items[i], entityList[i]);

            return success;
            //Blows up with "Method 'Void Add(T)' declared on type 'System.Collections.Generic.ICollection`1[T]' cannot be called with instance of type 'System.Collections.Generic.IEnumerable`1[T]'"
            //mapper.Map<IEnumerable<TData>, IEnumerable<TModel>>(items, entities).ToList();
        }

        internal static async Task<bool> DeleteAsync<TModel, TData>(this IStore store, IMapper mapper, Expression<Func<TModel, bool>> filter = null, IDictionary<Type, Type> typeMappings = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            Expression<Func<TData, bool>> f = mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
            List<TData> list = (await store.GetAsync(f)).ToList();
            list.ForEach(item => { item.EntityState = Data.EntityStateType.Deleted; });
            return await store.SaveAsync<TData>(list);
        }

        internal static void AddChanges<TModel, TData>(this IStore store, IMapper mapper, ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            store.AddChanges<TData>(mapper.Map<IEnumerable<TData>>(entities).ToList());
        }

        internal static void AddGraphChanges<TModel, TData>(this IStore store, IMapper mapper, ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            store.AddGraphChanges<TData>(mapper.Map<IEnumerable<TData>>(entities).ToList());
        }
    }
}
