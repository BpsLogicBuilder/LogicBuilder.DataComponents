using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class LessThanOrEqualsBinaryOperator : BinaryOperator
    {
        public LessThanOrEqualsBinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters, left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.le;
    }
}
