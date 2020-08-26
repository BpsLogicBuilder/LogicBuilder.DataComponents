using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class ConstantOperand : IExpressionPart
    {
        public ConstantOperand(Type type, object constantValue)
        {
            Type = type;
            ConstantValue = constantValue;
        }

        public ConstantOperand(object constantValue)
        {
            ConstantValue = constantValue;
        }

        public Type Type { get;  }
        public object ConstantValue { get; }

        public Expression Build() 
            => Type == null ? Expression.Constant(ConstantValue) : Expression.Constant(ConstantValue, Type);
    }
}
