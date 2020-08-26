using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators
{
    public class TotalSecondsOperator : IExpressionPart
    {
        public TotalSecondsOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type == typeof(TimeSpan))
                return Expression.Convert
                (
                    operandExpression.MakeSelector("TotalSeconds"),
                    typeof(decimal)
                );
            else
                throw new ArgumentException(nameof(Operand));
        }
    }
}
