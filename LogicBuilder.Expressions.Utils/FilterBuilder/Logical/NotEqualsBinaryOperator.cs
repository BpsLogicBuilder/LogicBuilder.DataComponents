using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class NotEqualsBinaryOperator : BinaryOperator
    {
        public NotEqualsBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.ne;

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
                    LinqHelpers.ByteArraysNotEqualMethodInfo
                );
            }

            return base.Build(left, right);
        }
    }
}
