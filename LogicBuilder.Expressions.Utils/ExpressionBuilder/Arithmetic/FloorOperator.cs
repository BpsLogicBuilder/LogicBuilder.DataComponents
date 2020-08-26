using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class FloorOperator : IExpressionPart
    {
        public FloorOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) => operandExpression.GetFloorCall();
    }
}
