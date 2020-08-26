using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical
{
    public class ConcatOperator : IExpressionPart
    {
        public ConcatOperator(IExpressionPart left, IExpressionPart right)
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
                return leftExpression.GetEnumerableConcatCall(Right.Build());
            else if (leftExpression.Type == typeof(string))
                return LinqHelpers.GetStringConcatCall(leftExpression, Right.Build());
            else
                throw new ArgumentException(nameof(leftExpression));
        }
    }
}
