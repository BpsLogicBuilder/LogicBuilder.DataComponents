using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.StringOperators
{
    public class ToLowerOperator : IExpressionPart
    {
        public ToLowerOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build()
        {
            Expression operandExpression = Operand.Build();

            if (operandExpression.Type == typeof(string))
                return operandExpression.GetStringToLowerCall();
            else
                throw new ArgumentException(nameof(Operand));
        }
    }
}
