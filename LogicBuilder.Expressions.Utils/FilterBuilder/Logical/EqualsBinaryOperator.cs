using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class EqualsBinaryOperator : BinaryOperator
    {
        public EqualsBinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters, left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.eq;

        protected override Expression Build(Expression left, Expression right)
        {
            if (left.Type == typeof(byte[]) || right.Type == typeof(byte[]))
            {
                left = left.SetNullType(typeof(byte[]));
                right = right.SetNullType(typeof(byte[]));

                return Expression.MakeBinary
                (
                    Constants.BinaryOperatorExpressionType[Operator], 
                    left, 
                    right, 
                    false,
                    LinqHelpers.ByteArraysEqualMethodInfo
                );
            }

            return base.Build(left, right);
        }
    }
}
