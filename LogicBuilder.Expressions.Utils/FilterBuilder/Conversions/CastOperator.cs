using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class CastOperator : FilterPart
    {
        public CastOperator(FilterPart operand, System.Type type)
        {
            Operand = operand;
            Type = type;
        }

        public FilterPart Operand { get; private set; }
        public System.Type Type { get; private set; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => Expression.TypeAs(operandExpression, Type);
    }
}
