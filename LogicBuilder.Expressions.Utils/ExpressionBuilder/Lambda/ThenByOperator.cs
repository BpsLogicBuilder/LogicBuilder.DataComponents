using LogicBuilder.Expressions.Utils.Strutures;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class ThenByOperator : IExpressionPart
    {
        public ThenByOperator(IExpressionPart sourceOperand, IExpressionPart selector, ListSortDirection sortDirection)
        {
            SourceOperand = sourceOperand;
            Selector = selector;
            SortDirection = sortDirection;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart Selector { get; }
        public ListSortDirection SortDirection { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetThenByCall((LambdaExpression)Selector.Build(), SortDirection);
    }
}
