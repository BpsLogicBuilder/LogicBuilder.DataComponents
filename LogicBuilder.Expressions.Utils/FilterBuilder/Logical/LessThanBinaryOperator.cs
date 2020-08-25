using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class LessThanBinaryOperator : BinaryOperator
    {
        public LessThanBinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters, left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.lt;
    }
}
