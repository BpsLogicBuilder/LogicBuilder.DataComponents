using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class ConvertToNumericDate : FilterPart
    {
        public ConvertToNumericDate(IDictionary<string, ParameterExpression> parameters, FilterPart sourceOperand) : base(parameters)
        {
            SourceOperand = sourceOperand;
        }

        public FilterPart SourceOperand { get; }

        public override Expression Build() => Build(SourceOperand.Build());

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
