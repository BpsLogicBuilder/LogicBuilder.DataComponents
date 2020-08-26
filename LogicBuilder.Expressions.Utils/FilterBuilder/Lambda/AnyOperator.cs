using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Lambda
{
    public class AnyOperator : FilterPart
    {
        public AnyOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand, FilterPart filter)
            : base(parameters)
        {
            Operand = operand;
            Filter = filter;
        }

        public AnyOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; }
        public FilterPart Filter { get; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetAnyQueryableCall(GetParameters())
                : operandExpression.GetAnyEnumerableCall(GetParameters());

        private Expression[] GetParameters()
                => Filter == null ? new Expression[0] : new Expression[] { Filter.Build() };
    }
}
