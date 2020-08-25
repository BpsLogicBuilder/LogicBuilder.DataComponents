using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.StringOperators
{
    public class ConvertToStringOperator : FilterPart
    {
        public ConvertToStringOperator(IDictionary<string, ParameterExpression> parameters, FilterPart sourceOperand) : base(parameters)
        {
            SourceOperand = sourceOperand;
        }

        public FilterPart SourceOperand { get; }

        public override Expression Build() => Build(SourceOperand.Build());

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
