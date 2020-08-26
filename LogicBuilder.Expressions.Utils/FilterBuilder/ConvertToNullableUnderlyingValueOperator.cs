using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ConvertToNullableUnderlyingValueOperator : FilterPart
    {
        public ConvertToNullableUnderlyingValueOperator(FilterPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public FilterPart SourceOperand { get; }

        public override Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (!operandExpression.Type.IsNullableType())
                throw new ArgumentException($"Unsupported expression type: {operandExpression.Type.Name}.  The type must be nullable.");

            return operandExpression.MakeValueSelectorAccessIfNullable();
        }
    }
}
