using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class BinaryOperatorHandler
    {
        public BinaryOperatorHandler(IExpressionPart left, IExpressionPart right, FilterFunction @operator)
        {
            Left = left;
            Right = right;
            Operator = @operator;
        }

        public virtual FilterFunction Operator { get; }
        public IExpressionPart Left { get; }
        public IExpressionPart Right { get; }

        public virtual Expression Build()
        {
            var left = Left.Build();
            var right = Right.Build();

            MatchTypes(ref left, ref right);

            return Build(left, right);
        }

        protected virtual Expression Build(Expression left, Expression right)
            => Expression.MakeBinary
            (
                Constants.BinaryOperatorExpressionType[Operator],
                left,
                right
            );

        protected void MatchTypes(ref Expression left, ref Expression right)
        {
            if (left.Type == right.Type)
                return;

            left = ToNullable(left);
            right = ToNullable(right);
        }

        private Expression ToNullable(Expression expression)
        {
            if (expression.Type.IsValueType && !expression.Type.IsNullableType())
                return Expression.Convert(expression, expression.Type.ToNullable());

            return expression;
        }
    }
}
