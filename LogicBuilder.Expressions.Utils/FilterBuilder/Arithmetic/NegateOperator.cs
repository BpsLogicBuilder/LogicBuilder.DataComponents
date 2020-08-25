using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class NegateOperator : FilterPart
    {
        public NegateOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            this.Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build()
            => Expression.Negate(this.Operand.Build());
    }
}
