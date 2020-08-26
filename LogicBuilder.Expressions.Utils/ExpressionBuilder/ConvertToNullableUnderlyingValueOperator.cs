using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class ConvertToNullableUnderlyingValueOperator : IExpressionPart
    {
        public ConvertToNullableUnderlyingValueOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (!operandExpression.Type.IsNullableType())
                throw new ArgumentException($"Unsupported expression type: {operandExpression.Type.Name}.  The type must be nullable.");

            return operandExpression.MakeValueSelectorAccessIfNullable();
        }
    }
}
