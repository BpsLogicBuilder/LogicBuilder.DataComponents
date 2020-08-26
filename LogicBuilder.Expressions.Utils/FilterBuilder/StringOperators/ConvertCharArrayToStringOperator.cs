using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.StringOperators
{
    public class ConvertCharArrayToStringOperator : FilterPart
    {
        public ConvertCharArrayToStringOperator(FilterPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public FilterPart SourceOperand { get; }

        public override Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (operandExpression.Type != typeof(char[]))
                throw new ArgumentException($"Unsupported expression type: {operandExpression.Type.Name}.  The type must be {typeof(char[]).FullName}.");

            return Expression.New(LinqHelpers.StringConstructorWithCharArrayParameters, operandExpression);
        }
    }
}
