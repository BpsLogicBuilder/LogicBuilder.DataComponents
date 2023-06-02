using Contoso.Data.Entities;
using Contoso.Domain;
using Kendo.Mvc;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using LogicBuilder.Data;
using LogicBuilder.Domain;
using LogicBuilder.EntityFrameworkCore.SqlServer.Repositories;
using LogicBuilder.Expressions.EntityFrameworkCore;
using LogicBuilder.Expressions.Utils;
using LogicBuilder.Expressions.Utils.Expansions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests.Models;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LogicBuilder.Kendo.ExpressionExtensions.IntegrationTests
{
    internal static class Helpers
    {
        public static async Task<DataSourceResult> GetData<TModel, TData>(this DataRequest request, IContextRepository contextRepository)
            where TModel : BaseModelClass
            where TData : BaseDataClass
        {
            return await request.Options.CreateDataSourceRequest().GetDataSourceResult<TModel, TData>
            (
                contextRepository,
                request.SelectExpandDefinition,
                request.Includes.BuildIncludesExpressionCollection<TModel>()
            );
        }

        public static async Task<IEnumerable<dynamic>> GetDynamicSelect<TModel, TData>(this DataRequest request, IContextRepository contextRepository)
            where TModel : BaseModelClass
            where TData : BaseDataClass
        {
            Expression<Func<IQueryable<TModel>, IEnumerable<dynamic>>> exp = Expression.Parameter(typeof(IQueryable<TModel>), "q").BuildLambdaExpression<IQueryable<TModel>, IEnumerable<dynamic>>
            (
                p => request.Distinct
                    ? request.Options.CreateDataSourceRequest()
                        .CreateUngroupedMethodExpression(p.GetSelectNew<TModel>(request.Selects))
                        .GetDistinct()
                    : request.Options.CreateDataSourceRequest()
                        .CreateUngroupedMethodExpression(p.GetSelectNew<TModel>(request.Selects))
            );

            return await contextRepository.QueryAsync<TModel, TData, IEnumerable<dynamic>, IEnumerable<dynamic>>
            (
                exp,
                request.SelectExpandDefinition
            );
        }

        public static async Task<TModel> GetSingle<TModel, TData>(this DataRequest request, IContextRepository contextRepository)
            where TModel : BaseModelClass
            where TData : BaseDataClass
        {

            Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> exp = request.Options.CreateDataSourceRequest().CreateUngroupedQueryableExpression<TModel>();

            return 
            (
                await contextRepository.QueryAsync<TModel, TData, IQueryable<TModel>, IQueryable<TData>>
                (
                    exp,
                    request.SelectExpandDefinition
                )
            ).Single();
        }

        public static DataSourceRequest CreateDataSourceRequest(this DataSourceRequestOptions req)
        {
            var request = new DataSourceRequest();

            if (req.Sort != null)
                request.Sorts = DataSourceDescriptorSerializer.Deserialize<SortDescriptor>(req.Sort);


            request.Page = req.Page;

            request.PageSize = req.PageSize;

            if (req.Filter != null)
                request.Filters = FilterDescriptorFactory.Create(req.Filter);

            if (req.Group != null)
                request.Groups = DataSourceDescriptorSerializer.Deserialize<GroupDescriptor>(req.Group);

            if (req.Aggregate != null)
                request.Aggregates = DataSourceDescriptorSerializer.Deserialize<AggregateDescriptor>(req.Aggregate);

            return request;
        }

        /// <summary>
        /// Get DataSource Result
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TData"></typeparam>
        /// <param name="request"></param>
        /// <param name="contextRepository"></param>
        /// <param name="selectExpandDefinition"></param>
        /// <returns></returns>
        public static async Task<DataSourceResult> GetDataSourceResult<TModel, TData>(this DataSourceRequest request, IContextRepository contextRepository, SelectExpandDefinition selectExpandDefinition = null, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
            => request.Groups != null && request.Groups.Count > 0
                ? await request.GetGroupedDataSourceResult<TModel, TData>(contextRepository, request.Aggregates != null && request.Aggregates.Count > 0, selectExpandDefinition, includeProperties)
                : await request.GetUngroupedDataSourceResult<TModel, TData>(contextRepository, request.Aggregates != null && request.Aggregates.Count > 0, selectExpandDefinition);

        private static async Task<DataSourceResult> GetUngroupedDataSourceResult<TModel, TData>(this DataSourceRequest request, IContextRepository contextRepository, bool getAggregates, SelectExpandDefinition selectExpandDefinition = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            Expression<Func<IQueryable<TModel>, AggregateFunctionsGroup>> aggregatesExp = getAggregates ? QueryableExtensionsEx.CreateAggregatesExpression<TModel>(request) : null;
            Expression<Func<IQueryable<TModel>, int>> totalExp = QueryableExtensionsEx.CreateTotalExpression<TModel>(request);
            Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> ungroupedExp = QueryableExtensionsEx.CreateUngroupedQueryableExpression<TModel>(request);

            return new DataSourceResult
            {
                Data = await contextRepository.QueryAsync<TModel, TData, IQueryable<TModel>, IQueryable<TData>>(ungroupedExp, selectExpandDefinition),
                AggregateResults = getAggregates
                                    ? (await contextRepository.QueryAsync<TModel, TData, AggregateFunctionsGroup, AggregateFunctionsGroup, AggregateFunctionsGroupModel<TModel>>(aggregatesExp, selectExpandDefinition))
                                                                ?.GetAggregateResults(request.Aggregates.SelectMany(a => a.Aggregates))
                                    : null,
                Total = await contextRepository.QueryAsync<TModel, TData, int, int>(totalExp, selectExpandDefinition)
            };
        }

        private static async Task<DataSourceResult> GetGroupedDataSourceResult<TModel, TData>(this DataSourceRequest request, IContextRepository contextRepository, bool getAggregates, SelectExpandDefinition selectExpandDefinition = null, ICollection<Expression<Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>>> includeProperties = null)
            where TModel : BaseModel
            where TData : BaseData
        {
            Expression<Func<IQueryable<TModel>, AggregateFunctionsGroup>> aggregatesExp = getAggregates ? QueryableExtensionsEx.CreateAggregatesExpression<TModel>(request) : null;
            Expression<Func<IQueryable<TModel>, int>> totalExp = QueryableExtensionsEx.CreateTotalExpression<TModel>(request);

            return new DataSourceResult
            {
                Data = await GetData(),
                AggregateResults = getAggregates
                                    ? (await contextRepository.QueryAsync<TModel, TData, AggregateFunctionsGroup, AggregateFunctionsGroup, AggregateFunctionsGroupModel<TModel>>(aggregatesExp, selectExpandDefinition))
                                                                ?.GetAggregateResults(request.Aggregates.SelectMany(a => a.Aggregates))
                                    : null,
                Total = await contextRepository.QueryAsync<TModel, TData, int, int>(totalExp, selectExpandDefinition)
            };

            async Task<IEnumerable> GetData()
            {
                var groupByExpressions = request.CreateGroupedByQueryExpressions<TModel>();
                IQueryable<TModel> pagedQuery = await contextRepository.QueryAsync<TModel, TData, IQueryable<TModel>, IQueryable<TData>>(groupByExpressions.PagingExpression, selectExpandDefinition, includeProperties);
                return groupByExpressions.GroupByExpression.Compile()(pagedQuery);
            }
        }
    }
}
