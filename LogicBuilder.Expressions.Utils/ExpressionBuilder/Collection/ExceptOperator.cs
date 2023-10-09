using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection
{
    public class ExceptOperator : IExpressionPart
    {
        public ExceptOperator(IExpressionPart left, IExpressionPart right)
        {
            Left = left;
            Right = right;
        }

        public IExpressionPart Left { get; private set; }
        public IExpressionPart Right { get; private set; }

        public Expression Build() => Build(Left.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type.IsList())
                return leftExpression.GetExceptCall(Right.Build());
            else
                throw new ArgumentException(nameof(Left));
        }
    }
}
