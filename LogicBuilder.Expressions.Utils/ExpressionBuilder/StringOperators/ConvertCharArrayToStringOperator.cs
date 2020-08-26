using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.StringOperators
{
    public class ConvertCharArrayToStringOperator : IExpressionPart
    {
        public ConvertCharArrayToStringOperator(IExpressionPart sourceOperand)
        {
            SourceOperand = sourceOperand;
        }

        public IExpressionPart SourceOperand { get; }

        public Expression Build() => Build(SourceOperand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (operandExpression.Type != typeof(char[]))
                throw new ArgumentException($"Unsupported expression type: {operandExpression.Type.Name}.  The type must be {typeof(char[]).FullName}.");

            return Expression.New(LinqHelpers.StringConstructorWithCharArrayParameters, operandExpression);
        }
    }
}
