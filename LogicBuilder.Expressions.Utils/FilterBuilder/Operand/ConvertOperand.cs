using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class ConvertOperand : FilterPart
    {
        public ConvertOperand(IDictionary<string, ParameterExpression> parameters, Type type, FilterPart sourceOperand) : base(parameters)
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
