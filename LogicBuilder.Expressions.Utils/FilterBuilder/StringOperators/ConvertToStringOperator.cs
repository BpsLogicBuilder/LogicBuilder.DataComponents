using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.StringOperators
{
    public class ConvertToStringOperator : IExpressionPart
    {
        public ConvertToStringOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (operandExpression.Type.IsNullableType())
                return ConvertNullable(operandExpression, operandExpression.MakeValueSelectorAccessIfNullable());

            return ConvertNonNullable(operandExpression);
        }

        private Expression ConvertNullable(Expression operandExpression, Expression underlyingExpression)
        {
            if (underlyingExpression.Type.IsEnum)
                underlyingExpression = ConvertEnumToUnderlyingType(underlyingExpression);

            return Expression.Condition
            (
                operandExpression.MakeHasValueSelector(),
                underlyingExpression.GetObjectToStringCall(),
                Expression.Constant(null, typeof(string))
            );
        }

        private Expression ConvertNonNullable(Expression operandExpression)
        {
            if (operandExpression.Type.IsEnum)
                operandExpression = ConvertEnumToUnderlyingType(operandExpression);

            return operandExpression.GetObjectToStringCall();
        }

        private Expression ConvertEnumToUnderlyingType(Expression expression)
            => Expression.Convert(expression, Enum.GetUnderlyingType(expression.Type));
    }
}
