using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class LengthOperator : FilterPart
    {
        public LengthOperator(FilterPart operand)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build() => Build(Operand.Build());

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
