using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class GreaterThanLessThanBinaryOperatorHandler : BinaryOperatorHandler
    {
        public GreaterThanLessThanBinaryOperatorHandler(IExpressionPart left, IExpressionPart right, FilterFunction @operator) : base(left, right, @operator)
        {
        }

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
