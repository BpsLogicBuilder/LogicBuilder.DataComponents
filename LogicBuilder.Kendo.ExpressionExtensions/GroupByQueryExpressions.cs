using Kendo.Mvc.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions
{
    public class GroupByQueryExpressions<TModel>
    {
        public GroupByQueryExpressions()
        {
        }

        public GroupByQueryExpressions(Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> pagingExpression, Expression<Func<IQueryable<TModel>, IEnumerable<AggregateFunctionsGroup>>> groupByExpression)
        {
            PagingExpression = pagingExpression;
            GroupByExpression = groupByExpression;
        }

        public Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> PagingExpression { get; set; }
        public Expression<Func<IQueryable<TModel>, IEnumerable<AggregateFunctionsGroup>>> GroupByExpression { get; set; }
    }
}
