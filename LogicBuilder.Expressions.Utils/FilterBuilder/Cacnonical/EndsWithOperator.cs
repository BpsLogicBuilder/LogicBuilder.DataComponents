using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class EndsWithOperator : IExpressionPart
    {
        public EndsWithOperator(IExpressionPart left, IExpressionPart right)
        {
            Left = left;
            Right = right;
        }

        public IExpressionPart Left { get; private set; }
        public IExpressionPart Right { get; private set; }

        public Expression Build() => Build(Left.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type == typeof(string))
                return leftExpression.GetStringEndsWithCall(Right.Build());
            else
                throw new ArgumentException(nameof(Left));
        }
    }
}
