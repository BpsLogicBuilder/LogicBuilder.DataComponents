using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    abstract public class BinaryOperator : FilterPart
    {
        public BinaryOperator(IDictionary<string, ParameterExpression> parameters, FilterPart left, FilterPart right) : base(parameters)
        {
            Left = left;
            Right = right;
        }

        public abstract FilterFunction Operator { get; }
        public FilterPart Left { get; }
        public FilterPart Right { get; }

        public override Expression Build()
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
