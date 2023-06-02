using Kendo.Mvc.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions
{
    public class AggregatesQueryExpressions<TModel>
    {
        public AggregatesQueryExpressions()
        {
        }

        public AggregatesQueryExpressions(Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> queryableExpression, Expression<Func<IQueryable<TModel>, AggregateFunctionsGroup>> aggregateExpression)
        {
            QueryableExpression = queryableExpression;
            AggregateExpression = aggregateExpression;
        }

        public Expression<Func<IQueryable<TModel>, IQueryable<TModel>>> QueryableExpression { get; set; }
        public Expression<Func<IQueryable<TModel>, AggregateFunctionsGroup>> AggregateExpression { get; set; }
    }
}
