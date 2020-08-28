using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Collection
{
    public class AsQueryableOperator : IExpressionPart
    {
        public AsQueryableOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetAsQueryableCall();
    }
}
