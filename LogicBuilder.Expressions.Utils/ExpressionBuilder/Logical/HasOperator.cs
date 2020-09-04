using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class HasOperator : IExpressionPart
    {
        public HasOperator(IExpressionPart left, IExpressionPart right)
        {
            Left = left;
            Right = right;
        }

        public IExpressionPart Left { get; private set; }
        public IExpressionPart Right { get; private set; }

        public Expression Build()
        {
            var left = Left.Build();
            
            return left.GetHasFlagCall
            (
                ConvertRightToEnumExpression
                (
                    Right.Build(),
                    left.Type.ToNullableUnderlyingType()
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
