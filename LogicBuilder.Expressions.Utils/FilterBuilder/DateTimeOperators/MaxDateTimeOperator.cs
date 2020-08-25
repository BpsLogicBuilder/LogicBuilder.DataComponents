using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class MaxDateTimeOperator : FilterPart
    {
        public MaxDateTimeOperator(IDictionary<string, ParameterExpression> parameters) : base(parameters)
        {
        }

        public override Expression Build() => LinqHelpers.GetMaxDateTimOffsetField();
    }
}
