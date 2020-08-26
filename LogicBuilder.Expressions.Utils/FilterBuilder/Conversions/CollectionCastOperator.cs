using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Conversions
{
    public class CollectionCastOperator : IExpressionPart
    {
        public CollectionCastOperator(IExpressionPart operand, Type type)
        {
            Operand = operand;
            Type = type;
        }

        public IExpressionPart Operand { get; private set; }
        public Type Type { get; private set; }

        public Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression) 
            => operandExpression.Type.IsIQueryable()
                ? operandExpression.GetOfTypeQueryableCall(Type)
                : operandExpression.GetOfTypeEnumerableCall(Type);
    }
}
