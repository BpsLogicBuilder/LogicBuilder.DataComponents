using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection
{
    public class SkipOperator : IExpressionPart
    {
        public SkipOperator(IExpressionPart sourceOperand, int count)
        {
            SourceOperand = sourceOperand;
            Count = count;
        }

        public IExpressionPart SourceOperand { get; }
        public int Count { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetSkipCall(Count);
    }
}
