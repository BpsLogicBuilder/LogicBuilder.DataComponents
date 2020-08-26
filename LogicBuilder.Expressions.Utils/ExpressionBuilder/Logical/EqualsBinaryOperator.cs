using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class EqualsBinaryOperator : BinaryOperator
    {
        public EqualsBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
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
