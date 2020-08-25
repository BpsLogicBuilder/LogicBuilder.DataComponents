using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class CollectionCastOperator : FilterPart
    {
        public CollectionCastOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand, Type type) : base(parameters)
        {
            Operand = operand;
            Type = type;
        }

        public FilterPart Operand { get; private set; }
        public Type Type { get; private set; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetOfTypeQueryableCall(Type)
                : operandExpression.GetOfTypeEnumerableCall(Type);
    }
}
