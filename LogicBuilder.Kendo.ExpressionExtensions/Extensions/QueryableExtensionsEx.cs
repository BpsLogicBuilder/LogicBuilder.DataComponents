using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.Infrastructure.Implementation.Expressions;
using Kendo.Mvc.UI;
using LogicBuilder.Kendo.ExpressionExtensions.Grouping;
using LogicBuilder.Kendo.ExpressionExtensions.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Extensions
{
    public static class QueryableExtensionsEx
    {
        /// <summary>
        /// Create Aggregates Expression
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<TModel>, AggregateFunctionsGroup>> CreateAggregatesExpression<TModel>(this DataSourceRequest request)
        {
            if (request.Aggregates == null || request.Aggregates.Count == 0)
                throw new ArgumentException("Aggregates are required.");

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<TModel>), "q");
            Expression ex = param;

            var filters = new List<IFilterDescriptor>();
            if (request.Filters != null)
                filters.AddRange(request.Filters);

            var aggregates = new List<AggregateDescriptor>(request.Aggregates);

            if (filters.Any())
                ex = ex.Where(filters);

            ex = ex.Aggregate(aggregates.SelectMany(a => a.Aggregates));
            ex = Expression.Call(typeof(Queryable), "FirstOrDefault", new Type[] { typeof(AggregateFunctionsGroup) }, ex);
            return Expression.Lambda<Func<IQueryable<TModel>, AggregateFunctionsGroup>>(ex, param);
        }

        /// <summary>
        /// Create Grouped Data Expression
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<TModel>, IEnumerable<AggregateFunctionsGroup>>> CreateGroupedDataExpression<TModel>(this DataSourceRequest request)
        {
            if (request.Groups == null || request.Groups.Count == 0)
                throw new ArgumentException("Groups are required.");

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<TModel>), "q");
            Expression ex = CreateGroupedMethodExpression(request, param);

            ex = Expression.Call(typeof(Enumerable), "ToList", new Type[] { typeof(AggregateFunctionsGroup) }, ex);

            return Expression.Lambda<Func<IQueryable<TModel>, IEnumerable<AggregateFunctionsGroup>>>(ex, param);
        }

        public static Expression CreateGroupedMethodExpression(this DataSourceRequest request, Expression ex)
        {
            if (request.Groups == null || request.Groups.Count == 0)
                throw new ArgumentException("Groups are required.");

            var filters = new List<IFilterDescriptor>();
            if (request.Filters != null)
                filters.AddRange(request.Filters);

            if (filters.Any())
                ex = ex.Where(filters);

            var sort = new List<SortDescriptor>();
            if (request.Sorts != null)
                sort.AddRange(request.Sorts);

            var temporarySortDescriptors = new List<SortDescriptor>();
            IList<GroupDescriptor> groups = new List<GroupDescriptor>(request.Groups);

            var aggregates = new List<AggregateDescriptor>();
            if (request.Aggregates != null)
                aggregates.AddRange(request.Aggregates);

            if (aggregates.Any())
                groups.Each(g =>
                {
                    g.AggregateFunctions.Clear();
                    g.AggregateFunctions.AddRange(aggregates.SelectMany(a => a.Aggregates));
                });

            if (!sort.Any())
            {
                // The Entity Framework provider demands OrderBy before calling Skip.
                SortDescriptor sortDescriptor = new SortDescriptor
                {
                    Member = ex.GetUnderlyingElementType().FirstSortableProperty()
                };
                sort.Add(sortDescriptor);
                temporarySortDescriptors.Add(sortDescriptor);
            }

            groups.Reverse().Each(groupDescriptor =>
            {
                var sortDescriptor = new SortDescriptor
                {
                    Member = groupDescriptor.Member,
                    SortDirection = groupDescriptor.SortDirection
                };

                sort.Insert(0, sortDescriptor);
                temporarySortDescriptors.Add(sortDescriptor);
            });

            ex = ex.GetSortExpression(sort);

            var notPagedData = ex;
            ex = ex.GetPageExpression(request.Page - 1, request.PageSize);
            ex = ex.GetGroupByExpression(notPagedData, groups);
            //ex = Expression.Call(typeof(Enumerable), "ToList", new Type[] { typeof(AggregateFunctionsGroup) }, ex);

            temporarySortDescriptors.Each(sortDescriptor => sort.Remove(sortDescriptor));

            return ex;
        }

        /// <summary>
        /// Create Ungrouped Data Expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<TSource>, IEnumerable<TSource>>> CreateUngroupedDataExpression<TSource>(this DataSourceRequest request)
        {
            if (request.Groups != null && request.Groups.Count > 0)
                throw new ArgumentException("Groups count must be zero.");

            ParameterExpression param = Expression.Parameter(typeof(IQueryable<TSource>), "q");
            Expression ex = CreateUngroupedMethodExpression(request, param);

            ex = Expression.Call(typeof(Enumerable), "ToList", new Type[] { typeof(TSource) }, ex);

            return Expression.Lambda<Func<IQueryable<TSource>, IEnumerable<TSource>>>(ex, param);
        }

        public static Expression CreateUngroupedMethodExpression(this DataSourceRequest request, Expression ex)
        {
            if (request.Groups != null && request.Groups.Count > 0)
                throw new ArgumentException("Groups count must be zero.");

            //ParameterExpression param = Expression.Parameter(typeof(IQueryable<TSource>), "q");
            //Expression ex = param;

            var filters = new List<IFilterDescriptor>();
            if (request.Filters != null)
                filters.AddRange(request.Filters);

            if (filters.Any())
                ex = ex.Where(filters);

            var sort = new List<SortDescriptor>();
            if (request.Sorts != null)
                sort.AddRange(request.Sorts);

            var temporarySortDescriptors = new List<SortDescriptor>();

            if (!sort.Any())
            {
                // The Entity Framework provider demands OrderBy before calling Skip.
                SortDescriptor sortDescriptor = new SortDescriptor
                {
                    Member = ex.GetUnderlyingElementType().FirstSortableProperty()
                };
                sort.Add(sortDescriptor);
                temporarySortDescriptors.Add(sortDescriptor);
            }

            ex = ex.GetSortExpression(sort);
            ex = ex.GetPageExpression(request.Page - 1, request.PageSize);

            temporarySortDescriptors.Each(sortDescriptor => sort.Remove(sortDescriptor));

            return ex;
        }

        /// <summary>
        /// Create Total Expression
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        public static Expression<Func<IQueryable<TSource>, int>> CreateTotalExpression<TSource>(this DataSourceRequest request)
        {
            ParameterExpression param = Expression.Parameter(typeof(IQueryable<TSource>), "q");
            Expression ex = param;

            var filters = new List<IFilterDescriptor>();
            if (request.Filters != null)
                filters.AddRange(request.Filters);

            if (filters.Any())
                ex = ex.Where(filters);

            ex = ex.Count();

            return Expression.Lambda<Func<IQueryable<TSource>, int>>(ex, param);
        }

        private static MethodCallExpression CallQueryableMethod(this Expression expression, string methodName, LambdaExpression selector)
            => Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { expression.GetUnderlyingElementType(), selector.Body.Type },
                expression,
                Expression.Quote(selector));

        /// <summary>
        /// Get Sort Expression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sortDescriptors"></param>
        /// <returns></returns>
        public static MethodCallExpression GetSortExpression(this Expression source, IEnumerable<SortDescriptor> sortDescriptors)
        {
            var builder = new SortDescriptorCollectionExpressionBuilderEx(source, sortDescriptors);
            return builder.GetSortExpression();
        }

        /// <summary>
        /// Get Page Expression
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Expression GetPageExpression(this Expression source, int pageIndex, int pageSize)
        {
            Expression query = source;

            query = query.Skip(pageIndex * pageSize);

            if (pageSize > 0)
            {
                query = query.Take(pageSize);
            }

            return query;
        }

        /// <summary>
        /// Select
        /// </summary>
        /// <param name="parentExpression"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static MethodCallExpression Select(this Expression parentExpression, LambdaExpression selector)
        {
            return parentExpression.CallQueryableMethod("Select", selector);
        }

        /// <summary>
        /// Group By
        /// </summary>
        /// <param name="parentExpression"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static MethodCallExpression GroupBy(this Expression parentExpression, LambdaExpression keySelector)
        {
            return parentExpression.CallQueryableMethod("GroupBy", keySelector);
        }

        /// <summary>
        /// Order By
        /// </summary>
        /// <param name="parentExpression"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static MethodCallExpression OrderBy(this Expression parentExpression, LambdaExpression keySelector)
        {
            return parentExpression.CallQueryableMethod("OrderBy", keySelector);
        }

        /// <summary>
        /// Order By Descending
        /// </summary>
        /// <param name="parentExpression"></param>
        /// <param name="keySelector"></param>
        /// <returns></returns>
        public static MethodCallExpression OrderByDescending(this Expression parentExpression, LambdaExpression keySelector)
        {
            return parentExpression.CallQueryableMethod("OrderByDescending", keySelector);
        }

        /// <summary>
        /// Order By
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="keySelector"></param>
        /// <param name="sortDirection"></param>
        /// <returns></returns>
        public static Expression OrderBy(this Expression expression, LambdaExpression keySelector, ListSortDirection? sortDirection)
        {
            if (sortDirection.HasValue)
            {
                if (sortDirection.Value == ListSortDirection.Ascending)
                {
                    return expression.OrderBy(keySelector);
                }

                return expression.OrderByDescending(keySelector);
            }

            return expression;
        }

        /// <summary>
        /// Get Group By Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="groupDescriptors"></param>
        /// <returns></returns>
        public static Expression GetGroupByExpression(this Expression expression, IEnumerable<GroupDescriptor> groupDescriptors)
        {
            return expression.GetGroupByExpression(expression, groupDescriptors);
        }

        /// <summary>
        /// Get Group By Expression
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="notPagedData"></param>
        /// <param name="groupDescriptors"></param>
        /// <returns></returns>
        public static Expression GetGroupByExpression(this Expression expression, Expression notPagedData, IEnumerable<GroupDescriptor> groupDescriptors)
        {
            var builder = new GroupDescriptorCollectionExpressionBuilderEx(expression, groupDescriptors, notPagedData);
            //builder.Options.LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider();
            builder.Options.LiftMemberAccessToNull = false;
            return builder.CreateExpression();
        }

        /// <summary>
        /// This method call expression will select a new instance of AggregateFunctionsGroup 
        /// i.e. Expression<Func<IQueryable<T>, IQueryable<T>>> ex = q => q.Select(new AggregateFunctionsGroup { });
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="aggregateFunctions"></param>
        /// <returns></returns>
        public static MethodCallExpression Aggregate(this Expression expression, IEnumerable<AggregateFunction> aggregateFunctions)
        {
            var functions = aggregateFunctions.ToList();

            if (functions.Count > 0)
            {
                var builder = new QueryableAggregatesExpressionBuilderEx(expression, functions);
                //builder.Options.LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider();
                builder.Options.LiftMemberAccessToNull = false;
                return builder.CreateExpression();
            }

            return null;
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Expression Where(this Expression expression, Expression predicate)
        {
            return Expression.Call(
                   typeof(Queryable),
                   "Where",
                   new[] { expression.GetUnderlyingElementType() },
                   expression,
                   Expression.Quote(predicate));
        }

        /// <summary>
        /// Where
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="filterDescriptors"></param>
        /// <param name="isLiftedToNull"></param>
        /// <returns></returns>
        public static Expression Where(this Expression expression, IEnumerable<IFilterDescriptor> filterDescriptors, bool isLiftedToNull = false)
        {
            if (filterDescriptors.Any())
            {
                var parameterExpression = Expression.Parameter(expression.GetUnderlyingElementType(), "item");

                var expressionBuilder = new FilterDescriptorCollectionExpressionBuilder(parameterExpression, filterDescriptors);
                //expressionBuilder.Options.LiftMemberAccessToNull = source.Provider.IsLinqToObjectsProvider();
                expressionBuilder.Options.LiftMemberAccessToNull = isLiftedToNull;
                var predicate = expressionBuilder.CreateFilterExpression();
                return expression.Where(predicate);
            }

            return expression;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Expression Take(this Expression expression, int count)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            return Expression.Call(
                    typeof(Queryable), "Take",
                    new Type[] { expression.GetUnderlyingElementType() },
                    expression, Expression.Constant(count));
        }

        /// <summary>
        /// Skip
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Expression Skip(this Expression expression, int count)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            return Expression.Call(
                    typeof(Queryable), "Skip",
                    new Type[] { expression.GetUnderlyingElementType() },
                    expression, Expression.Constant(count));
        }

        /// <summary>
        /// Count
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MethodCallExpression Count(this Expression expression)
        {
            if (expression == null) throw new ArgumentNullException("expression");
            return Expression.Call(
                    typeof(Queryable), "Count",
                    new Type[] { expression.GetUnderlyingElementType() }, expression);
        }
    }
}
