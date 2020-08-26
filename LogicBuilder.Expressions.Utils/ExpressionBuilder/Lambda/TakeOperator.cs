using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class TakeOperator : IExpressionPart
    {
        public TakeOperator(IExpressionPart sourceOperand, int count)
        {
            SourceOperand = sourceOperand;
            Count = count;
        }

        public IExpressionPart SourceOperand { get; }
        public int Count { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetTakeCall(Count);
    }
}
