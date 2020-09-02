using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Cacnonical
{
    public class IndexOfOperator : IExpressionPart
    {
        public IndexOfOperator(IExpressionPart sourceOperand, IExpressionPart itemToFind)
        {
            SourceOperand = sourceOperand;
            ItemToFind = itemToFind;
        }

        public IExpressionPart SourceOperand { get; private set; }
        public IExpressionPart ItemToFind { get; private set; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression leftExpression)
        {
            if (leftExpression.Type == typeof(string))
                return leftExpression.GetStringIndexOfCall(ItemToFind.Build());
            else
                throw new ArgumentException(nameof(SourceOperand));
        }
    }
}
