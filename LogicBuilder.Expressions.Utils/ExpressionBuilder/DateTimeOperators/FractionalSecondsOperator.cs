using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators
{
    public class FractionalSecondsOperator : IExpressionPart
    {
        public FractionalSecondsOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

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
