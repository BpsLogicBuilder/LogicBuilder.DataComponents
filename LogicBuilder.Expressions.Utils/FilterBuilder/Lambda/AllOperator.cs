using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Lambda
{
    public class AllOperator : FilterPart
    {
        public AllOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand, FilterPart filter)
            : base(parameters)
        {
            Operand = operand;
            Filter = filter;
        }

        public AllOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; }
        public FilterPart Filter { get; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetAllQueryableCall(GetParameters())
                : operandExpression.GetAllEnumerableCall(GetParameters());

        private Expression[] GetParameters()
                => Filter == null ? new Expression[0] : new Expression[] { Filter.Build() };
    }
}
