using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class NegateOperator : IExpressionPart
    {
        public NegateOperator(IExpressionPart operand)
        {
            this.Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build()
            => Expression.Negate(this.Operand.Build());
    }
}
