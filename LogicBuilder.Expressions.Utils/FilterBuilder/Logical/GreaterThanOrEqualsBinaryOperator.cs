using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class GreaterThanOrEqualsBinaryOperator : BinaryOperator
    {
        public GreaterThanOrEqualsBinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters, left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.ge;
    }
}
