using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class NowDateTimeOperator : FilterPart
    {
        public NowDateTimeOperator(IDictionary<string, ParameterExpression> parameters) : base(parameters)
        {
        }

        public override Expression Build() => LinqHelpers.GetNowDateTimOffsetProperty();
    }
}
