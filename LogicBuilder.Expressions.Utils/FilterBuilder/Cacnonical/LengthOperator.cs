using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class LengthOperator : IExpressionPart
    {
        public LengthOperator(IExpressionPart operand)
        {
            Operand = operand;
        }

        public IExpressionPart Operand { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
        {
            if (operandExpression.Type.IsList())
                return operandExpression.GetEnumerableCountCall();
            else if (operandExpression.Type == typeof(string))
                return operandExpression.MakeSelector("Length");
            else
                throw new ArgumentException(nameof(Operand));
        }
    }
}
