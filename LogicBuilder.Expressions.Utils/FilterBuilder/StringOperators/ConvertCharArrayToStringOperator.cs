using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.StringOperators
{
    public class ConvertCharArrayToStringOperator : FilterPart
    {
        public ConvertCharArrayToStringOperator(IDictionary<string, ParameterExpression> parameters, FilterPart sourceOperand) : base(parameters)
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
