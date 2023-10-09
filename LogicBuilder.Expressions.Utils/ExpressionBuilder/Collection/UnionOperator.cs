using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection
{
    public class UnionOperator : IExpressionPart
    {
        public UnionOperator(IExpressionPart left, IExpressionPart right)
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
                return leftExpression.GetUnionCall(Right.Build());
            else
                throw new ArgumentException(nameof(Left));
        }
    }
}
