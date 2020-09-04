using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public abstract class EqualityBinaryOperatorHandlerBase : BinaryOperatorHandler
    {
        public EqualityBinaryOperatorHandlerBase(IExpressionPart left, IExpressionPart right, FilterFunction @operator) : base(left, right, @operator)
        {
        }

        protected abstract MethodInfo CompareMethodInfo { get; }

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
                    CompareMethodInfo
                );
            }

            return base.Build(left, right);
        }
    }
}
