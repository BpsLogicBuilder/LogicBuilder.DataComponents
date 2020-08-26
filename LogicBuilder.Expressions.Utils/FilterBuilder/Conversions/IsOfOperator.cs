using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class IsOfOperator : IExpressionPart
    {
        public IsOfOperator(IExpressionPart operand, System.Type type)
        {
            Operand = operand;
            Type = type;
        }

        public IExpressionPart Operand { get; private set; }
        public System.Type Type { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
            => Expression.Condition
            (
                Expression.TypeIs(operandExpression, Type), 
                Expression.Constant(true), 
                Expression.Constant(false)
            );
    }
}
