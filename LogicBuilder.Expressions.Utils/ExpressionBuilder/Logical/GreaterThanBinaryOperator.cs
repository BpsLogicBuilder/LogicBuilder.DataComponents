using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class GreaterThanBinaryOperator : BinaryOperator
    {
        public GreaterThanBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.gt;

        protected override Expression Build(Expression left, Expression right)
        {
            if (left.Type == typeof(string) || right.Type == typeof(string))
            {
                return base.Build
                (
                    LinqHelpers.GetStringCompareCall(left.SetNullType(typeof(string)), right.SetNullType(typeof(string))), 
                    Expression.Constant(0)
                );
            }

            if (left.Type.ToNullableUnderlyingType() == typeof(Guid) || right.Type.ToNullableUnderlyingType() == typeof(Guid))
            {
                return base.Build
                (
                    LinqHelpers.GetGuidCopareCall(left.SetNullType(typeof(Guid)), right.SetNullType(typeof(Guid))),
                    Expression.Constant(0)
                );
            }

            return base.Build(left, right);
        }
    }
}