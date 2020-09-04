using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Operand
{
    public class ConstantOperator : IExpressionPart
    {
        public ConstantOperator(object constantValue, Type type)
        {
            Type = type;
            ConstantValue = constantValue;
        }

        public ConstantOperator(object constantValue)
        {
            ConstantValue = constantValue;
        }

        public Type Type { get;  }
        public object ConstantValue { get; }

        public Expression Build() 
            => Type == null ? Expression.Constant(ConstantValue) : Expression.Constant(ConstantValue, Type);
    }
}
