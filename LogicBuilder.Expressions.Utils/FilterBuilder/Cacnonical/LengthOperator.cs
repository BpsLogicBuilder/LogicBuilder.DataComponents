using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Cacnonical
{
    public class LengthOperator : FilterPart
    {
        public LengthOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
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
