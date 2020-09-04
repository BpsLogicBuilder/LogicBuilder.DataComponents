using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand
{
    public class ConvertOperator : IExpressionPart
    {
        public ConvertOperator(IExpressionPart sourceOperand, Type type)
        {
            Type = type;
            SourceOperand = sourceOperand;
        }

        public Type Type { get; }
        public IExpressionPart SourceOperand { get; }

        public Expression Build()
        {
            try
            {
                return Expression.Convert(SourceOperand.Build(), Type);
            }
            catch (InvalidOperationException)
            {
                return Expression.Constant(null);
            }
        }
    }
}
