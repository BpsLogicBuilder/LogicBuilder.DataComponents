using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.SqlServer.Crud.DataStores;
using LogicBuilder.EntityFrameworkCore.SqlServer.Visitors;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Expressions.Utils.Strutures;
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
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null,
            ICollection<FilteredIncludeExpression> filteredIncludes = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            //Map the expressions
            Expression<Func<TData, bool>> f = mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
            Expression<Func<IQueryable<TData>, IQueryable<TData>>> mappedQueryFunc = mapper.MapExpression<Expression<Func<IQueryable<TData>, IQueryable<TData>>>>(queryFunc);
            ICollection<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>> includes = mapper.MapIncludesList<Expression<Func<IQueryable<TData>, IIncludableQueryable<TData, object>>>>(includeProperties);

            //Call the store
            ICollection<TData> list = await store.GetAsync
            (
                f,
                mappedQueryFunc?.Compile(),
                includes?.Select(i => i.Compile()).ToList(),
                filteredIncludes.MapFilteredIncludes<TModel, TData>(mapper)
            );

            //Map and return the data
            return mapper.Map<IEnumerable<TData>, IEnumerable<TModel>>(list).ToList();
        }

        internal static async Task<ICollection<TModel>> GetAsync<TModel, TData>(this IStore store, IMapper mapper,
            Expression<Func<TModel, bool>> filter = null,
            Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryFunc = null,
            SelectExpandDefinition selectExpandDefinition = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            return mapper.ProjectTo
            (
                await store.GetQueryableAsync
                (
                    Getfilter(),
                    GetQueryFunc()
                ),
                null,
                GetIncludes()
            )
            .UpdateQueryable(selectExpandDefinition.GetExpansions(typeof(TModel)), mapper)
            .ToList();

            Expression<Func<TModel, object>>[] GetIncludes() 
                => selectExpandDefinition?.GetExpansionSelectors<TModel>().ToArray() ?? new Expression<Func<TModel, object>>[] { };

            Func<IQueryable<TData>, IQueryable<TData>> GetQueryFunc()
                => mapper.MapExpression<Expression<Func<IQueryable<TData>, IQueryable<TData>>>>(queryFunc)?.Compile();

            Expression<Func<TData, bool>> Getfilter()
                => mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
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

        public static async Task<TModelReturn> QueryAsync<TModel, TData, TModelReturn, TDataReturn>(this IStore store, IMapper mapper,
            Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
            => await store.QueryAsync<TModel, TData, TModelReturn, TDataReturn, TModelReturn>(mapper, queryFunc, includeProperties);

        public static async Task<TModelReturn> QueryAsync<TModel, TData, TModelReturn>(this IStore store, IMapper mapper,
            Expression<Func<IQueryable<TModel>, TModelReturn>> queryFunc,
            ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
            => await store.QueryAsync<TModel, TData, TModelReturn, TModelReturn, TModelReturn>(mapper, queryFunc, includeProperties);

        internal static async Task<int> CountAsync<TModel, TData>(this IStore store, IMapper mapper, Expression<Func<TModel, bool>> filter = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            Expression<Func<TData, bool>> f = mapper.MapExpression<Expression<Func<TData, bool>>>(filter);
            return await store.CountAsync(f);
        }

        internal static async Task<bool> SaveGraphsAsync<TModel, TData>(this IStore store, IMapper mapper, ICollection<TModel> entities)
            where TModel : BaseModel
            where TData : BaseData
        {
            IList<TData> items = mapper.Map<IEnumerable<TData>>(entities).ToList();
            bool success = await store.SaveGraphsAsync<TData>(items);

            IList<TModel> entityList = entities.ToList();
            for (int i = 0; i < items.Count; i++)
                mapper.Map<TData, TModel>(items[i], entityList[i]);

            return success;
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
        }

        internal static async Task<bool> DeleteAsync<TModel, TData>(this IStore store, IMapper mapper, Expression<Func<TModel, bool>> filter = null)
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

        internal static IQueryable<TModel> UpdateQueryable<TModel>(this IQueryable<TModel> query, List<List<ExpansionOptions>> expansions, IMapper mapper)
        {
            List<List<ExpansionOptions>> filters = GetFilters();
            List<List<ExpansionOptions>> methods = GetQueryMethods();

            if (!filters.Any() && !methods.Any())
                return query;

            Expression expression = query.Expression;

            if (methods.Any())
                expression = UpdateProjectionMethodExpression(expression);

            if (filters.Any())//do filter last so it runs before a Skip or Take call.
                expression = UpdateProjectionFilterExpression(expression);

            return query.Provider.CreateQuery<TModel>(expression);

            Expression UpdateProjectionFilterExpression(Expression projectionExpression)
            {
                filters.ForEach
                (
                    filterList => projectionExpression = FilterUpdater.UpdaterExpansion
                    (
                        projectionExpression,
                        filterList,
                        mapper
                    )
                );

                return projectionExpression;
            }

            Expression UpdateProjectionMethodExpression(Expression projectionExpression)
            {
                methods.ForEach
                (
                    methodList => projectionExpression = QueryFunctionUpdater.UpdaterExpansion
                    (
                        projectionExpression,
                        methodList,
                        mapper
                    )
                );

                return projectionExpression;
            }

            List<List<ExpansionOptions>> GetFilters()
                => expansions.Aggregate(new List<List<ExpansionOptions>>(), (listOfLists, nextList) =>
                {
                    var filterNextList = nextList.Aggregate(new List<ExpansionOptions>(), (list, next) =>
                    {
                        if (next.FilterOption != null)
                        {
                            list = list.ConvertAll
                            (
                                exp => new ExpansionOptions
                                {
                                    MemberName = exp.MemberName,
                                    MemberType = exp.MemberType,
                                    ParentType = exp.ParentType,
                                }
                            );//new list removing filter

                            list.Add
                            (
                                new ExpansionOptions
                                {
                                    MemberName = next.MemberName,
                                    MemberType = next.MemberType,
                                    ParentType = next.ParentType,
                                    FilterOption = new ExpansionFilterOption { Filter = next.FilterOption.Filter }
                                }
                            );//add expansion with filter

                            listOfLists.Add(list.ToList()); //Add the whole list to the list of filter lists
                                                            //Only the last item in each list has a filter
                                                            //Filters for parent expansions exist in their own lists
                            return list;
                        }

                        list.Add(next);

                        return list;
                    });

                    return listOfLists;
                });

            List<List<ExpansionOptions>> GetQueryMethods()
                => expansions.Aggregate(new List<List<ExpansionOptions>>(), (listOfLists, nextList) =>
                {
                    var filterNextList = nextList.Aggregate(new List<ExpansionOptions>(), (list, next) =>
                    {
                        if (next.QueryOption != null)
                        {
                            list = list.ConvertAll
                            (
                                exp => new ExpansionOptions
                                {
                                    MemberName = exp.MemberName,
                                    MemberType = exp.MemberType,
                                    ParentType = exp.ParentType,
                                }
                            );//new list removing query options

                            list.Add
                            (
                                new ExpansionOptions
                                {
                                    MemberName = next.MemberName,
                                    MemberType = next.MemberType,
                                    ParentType = next.ParentType,
                                    QueryOption = new ExpansionQueryOption { SortCollection = next.QueryOption.SortCollection }
                                    //QueryOption = new ExpansionQueryOption { QueryFunction = next.QueryOption.QueryFunction }
                                }
                            );//add expansion with query options

                            listOfLists.Add(list.ToList()); //Add the whole list to the list of query method lists
                                                            //Only the last item in each list has a query method
                                                            //Query methods for parent expansions exist in their own lists
                            return list;
                        }

                        list.Add(next);

                        return list;
                    });

                    return listOfLists;
                });
        }
    }
}
