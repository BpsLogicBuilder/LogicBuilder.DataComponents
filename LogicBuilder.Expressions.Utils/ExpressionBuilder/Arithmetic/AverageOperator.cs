using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class AverageOperator : IExpressionPart
    {
        public AverageOperator(IExpressionPart sourceOperand, IExpressionPart selector)
        {
            SourceOperand = sourceOperand;
            Selector = selector;
        }

        public AverageOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }
        public IExpressionPart Selector { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.GetAverageMethodCall(Selector == null ? new Expression[0] : new Expression[] { (LambdaExpression)Selector.Build() });
    }
}
