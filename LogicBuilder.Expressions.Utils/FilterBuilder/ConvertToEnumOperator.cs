using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ConvertToEnumOperator : FilterPart
    {
        public ConvertToEnumOperator(IDictionary<string, ParameterExpression> parameters, Type type, object constantValue) : base(parameters)
        {
            Type = type;
            ConstantValue = constantValue;
        }

        public Type Type { get; }
        public object ConstantValue { get; }

        public override Expression Build() => DoBuild();

        private Expression DoBuild()
        {
            if (ConstantValue == null)
                return Expression.Constant(null);

            return ConstantValue.ToString().TryParseEnum(Type, out object enumValue) 
                ? Expression.Constant(enumValue, Type) 
                : Expression.Constant(null);
        }
    }
}
