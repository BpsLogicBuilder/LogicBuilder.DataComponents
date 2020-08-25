using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class TotalSecondsOperator : FilterPart
    {
        public TotalSecondsOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build() => Build(Operand.Build());

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
