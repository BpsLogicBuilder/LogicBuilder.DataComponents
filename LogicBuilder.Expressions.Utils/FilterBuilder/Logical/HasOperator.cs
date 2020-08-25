using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class HasOperator : FilterPart
    {
        public HasOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters)
        {
            Left = left;
            Right = right;
        }

        public FilterPart Left { get; private set; }
        public FilterPart Right { get; private set; }

        public override Expression Build()
        {
            var left = Left.Build();
            
            return left.GetHasFlagCall
            (
                ConvertRightToEnumExpression
                (
                    Right.Build(),
                    left.Type.IsNullableType() ? Nullable.GetUnderlyingType(left.Type) : left.Type
                )
            );
        }

        private Expression ConvertRightToEnumExpression(Expression right, Type leftType)
        {
            if (!leftType.IsEnum)
                throw new ArgumentException(nameof(leftType));

            return Expression.Convert
            (
                Expression.Constant
                (
                    Enum.Parse
                    (
                        leftType,
                        ((ConstantExpression)right).Value.ToString()
                    ),
                    leftType
                ), 
                typeof(Enum)
            );
        }
    }
}
