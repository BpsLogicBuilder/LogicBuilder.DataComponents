using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class RoundOperator : IExpressionPart
    {
        public RoundOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) => operandExpression.GetRoundCall();
    }
}
