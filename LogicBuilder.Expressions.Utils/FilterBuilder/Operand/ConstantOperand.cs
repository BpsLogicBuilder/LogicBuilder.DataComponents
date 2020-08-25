using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Operand
{
    public class ConstantOperand : FilterPart
    {
        public ConstantOperand(IDictionary<string, ParameterExpression> parameters, Type type, object constantValue) : base(parameters)
        {
            Type = type;
            ConstantValue = constantValue;
        }

        public ConstantOperand(IDictionary<string, ParameterExpression> parameters, object constantValue) : base(parameters)
        {
            ConstantValue = constantValue;
        }

        public Type Type { get;  }
        public object ConstantValue { get; }

        public override Expression Build() 
            => Type == null ? Expression.Constant(ConstantValue) : Expression.Constant(ConstantValue, Type);
    }
}
