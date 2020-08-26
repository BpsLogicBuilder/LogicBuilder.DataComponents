using Microsoft.OData.Edm;
using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class ConvertToNumericDate : IExpressionPart
    {
        public ConvertToNumericDate(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type != typeof(DateTimeOffset) && operandExpression.Type != typeof(DateTime) && operandExpression.Type != typeof(Date))
                return operandExpression;

            return Expression.Add
            (
                Expression.Add
                (
                    Expression.Multiply
                    (
                        operandExpression.MakeSelector("Year"), 
                        Expression.Constant(10000)
                    ),
                    Expression.Multiply
                    (
                        operandExpression.MakeSelector("Month"), 
                        Expression.Constant(100)
                    )
                ),
                operandExpression.MakeSelector("Day")
            );
        }
    }
}
