using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class FloorOperator : FilterPart
    {
        public FloorOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) => operandExpression.GetFloorCall();
    }
}
