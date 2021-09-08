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

        public Expression Build() => GetConstantExpression(Type ?? ConstantValue?.GetType());

        private Expression GetConstantExpression(Type constantType)
        {
            if (constantType == null)
                return Expression.Constant(ConvertConstantValue());

            if (constantType.IsLiteralType() == false)
                return Expression.Constant(ConvertConstantValue(), constantType);

            return CreateExpression(typeof(ConstantContainer<>).MakeGenericType(constantType));

            Expression CreateExpression(Type containerType)
                => Expression.Property
                (
                    Expression.Constant
                    (
                        Activator.CreateInstance(containerType, ConvertConstantValue()),
                        containerType
                    ),
                    nameof(ConstantContainer<object>.TypedProperty)
                );
        }

        private object ConvertConstantValue()
        {
            if (Type == null || ConstantValue?.GetType() == Type)
                return ConstantValue;

            return Convert.ChangeType(ConstantValue, Type);
        }
    }
}
