using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class ConvertOperand : FilterPart
    {
        public ConvertOperand(Type type, FilterPart sourceOperand)
        {
            Type = type;
            SourceOperand = sourceOperand;
        }

        public Type Type { get; }
        public FilterPart SourceOperand { get; }

        public override Expression Build()
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
