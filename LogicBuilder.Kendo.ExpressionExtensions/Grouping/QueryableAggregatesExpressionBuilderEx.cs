using Kendo.Mvc;
using Kendo.Mvc.Extensions;
using LogicBuilder.Kendo.ExpressionExtensions.Extensions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Kendo.ExpressionExtensions.Grouping
{
    internal class QueryableAggregatesExpressionBuilderEx : GroupDescriptorExpressionBuilderEx
    {
        public QueryableAggregatesExpressionBuilderEx(Expression queryable, IEnumerable<AggregateFunction> aggregateFunctions)
            : base(queryable, CreateDescriptor(aggregateFunctions))
        {
        }

        private static GroupDescriptor CreateDescriptor(IEnumerable<AggregateFunction> aggregateFunctions)
        {
            var groupDescriptor = new GroupDescriptor();
            groupDescriptor.AggregateFunctions.AddRange(aggregateFunctions);

            return groupDescriptor;
        }

        protected override LambdaExpression CreateGroupByExpression()
        {
            return Expression.Lambda(Expression.Constant(1), this.ParameterExpression);
        }

        public override MethodCallExpression CreateExpression()
        {
            return
                queryable.
                    GroupBy(this.CreateGroupByExpression(), evaluateGroupByOnClient: false).
                    OrderBy(this.CreateOrderByExpression(), SortDirection).
                    Select(this.CreateSelectExpression());

        }

        protected override IEnumerable<MemberBinding> CreateMemberBindings()
        {
            yield return this.CreateKeyMemberBinding();
            yield return this.CreateCountMemberBinding();
            yield return this.CreateHasSubgroupsMemberBinding();
            if (GroupDescriptor.AggregateFunctions.Count > 0)
            {
                yield return this.CreateAggregateFunctionsProjectionMemberBinding();
            }
            yield return this.CreateFieldNameMemberBinding();
        }
    }
}
