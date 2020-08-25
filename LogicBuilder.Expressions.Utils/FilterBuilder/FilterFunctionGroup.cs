using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class FilterFunctionGroup : FilterPart
    {
        public GroupOperatorType GroupOperatorType { get; set; }
        public IList<FilterPart> Filters { get; set; }

        readonly IDictionary<GroupOperatorType, Func<Expression, Expression, BinaryExpression>> GroupOperatorFunctions = new Dictionary<GroupOperatorType, Func<Expression, Expression, BinaryExpression>>
        {
            [GroupOperatorType.and] = Expression.And,
            [GroupOperatorType.or] = Expression.Or
        };

        public FilterFunctionGroup(IDictionary<string, ParameterExpression> parameters) : base(parameters)
        {
        }

        public override Expression Build()
        {
            if (!(Filters?.Any() == true))
                return null;

            if (Filters?.Count == 1)
                return Filters[0].Build();

            return Filters.Skip(1).Aggregate
            (
                Filters[0].Build(),
                DoAggregation
            );
        }

        Expression DoAggregation(Expression expression, FilterPart filterPart)
                => GroupOperatorFunctions[GroupOperatorType](expression, filterPart.Build());
    }
}
