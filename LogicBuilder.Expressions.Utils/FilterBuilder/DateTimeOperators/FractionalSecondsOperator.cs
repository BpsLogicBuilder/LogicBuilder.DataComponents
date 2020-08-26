using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class FractionalSecondsOperator : FilterPart
    {
        public FractionalSecondsOperator(FilterPart operand)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => GetFractionalSeconds(operandExpression.MakeMillisecondSelector());

        private Expression GetFractionalSeconds(Expression milliseconds) 
            => Expression.Divide
            (
                Expression.Convert(milliseconds, typeof(decimal)),
                Expression.Constant(1000m, typeof(decimal))
            );
    }
}
