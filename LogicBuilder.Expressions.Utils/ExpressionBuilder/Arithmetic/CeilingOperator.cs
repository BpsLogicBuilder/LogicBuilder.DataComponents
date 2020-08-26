using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class CeilingOperator : IExpressionPart
    {
        public CeilingOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) => operandExpression.GetCeilingCall();
    }
}
