using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ConvertToEnumOperator : FilterPart
    {
        public ConvertToEnumOperator(Type type, object constantValue)
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
