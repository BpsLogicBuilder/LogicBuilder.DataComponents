using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class CastOperator : IExpressionPart
    {
        public CastOperator(IExpressionPart operand, System.Type type)
        {
            Operand = operand;
            Type = type;
        }

        public IExpressionPart Operand { get; private set; }
        public System.Type Type { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => Expression.TypeAs(operandExpression, Type);
    }
}
