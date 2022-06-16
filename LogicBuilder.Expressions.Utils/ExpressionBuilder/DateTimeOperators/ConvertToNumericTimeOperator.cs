using Microsoft.OData.Edm;
using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators
{
    public class ConvertToNumericTimeOperator : IExpressionPart
    {
        public ConvertToNumericTimeOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            operandExpression = operandExpression.MakeValueSelectorAccessIfNullable();

            if (operandExpression.Type != typeof(DateTimeOffset) 
                && operandExpression.Type != typeof(DateTime) 
                && operandExpression.Type != typeof(TimeSpan)
                && operandExpression.Type != typeof(TimeOfDay)
                && operandExpression.Type.FullName != NET6OnlyLiteralTypeNames.TIMEONLY)
                return operandExpression;

            return Expression.Add
            (
                Expression.Multiply
                (
                    Expression.Convert(operandExpression.MakeHourSelector(), typeof(long)),
                    Expression.Constant(TimeSpan.TicksPerHour)
                ),
                Expression.Add
                (
                    
                    Expression.Multiply
                    (
                        Expression.Convert(operandExpression.MakeMinuteSelector(), typeof(long)), 
                        Expression.Constant(TimeSpan.TicksPerMinute)
                    ),
                    Expression.Add
                    (
                        Expression.Multiply
                        (
                            Expression.Convert(operandExpression.MakeSecondSelector(), typeof(long)),
                            Expression.Constant(TimeSpan.TicksPerSecond)
                        ),
                        Expression.Convert(operandExpression.MakeMillisecondSelector(), typeof(long))
                    )
                )
            );
        }
    }
}
