using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class NotOperator : FilterPart
    {
        public NotOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            this.Operand = operand;
        }

        public FilterPart Operand { get; private set; }

        public override Expression Build() 
            => Expression.Not(this.Operand.Build());
    }
}
