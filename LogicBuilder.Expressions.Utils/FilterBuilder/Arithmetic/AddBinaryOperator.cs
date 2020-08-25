using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class AddBinaryOperator : BinaryOperator
    {
        public AddBinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters, left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.add;
    }
}