using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class CollectionCastOperator : FilterPart
    {
        public CollectionCastOperator(FilterPart operand, Type type)
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
