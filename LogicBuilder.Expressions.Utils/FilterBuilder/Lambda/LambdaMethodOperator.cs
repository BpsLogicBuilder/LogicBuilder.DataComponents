using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Lambda
{
    public abstract class LambdaMethodOperator : FilterPart
    {
        public LambdaMethodOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand, FilterPart filter, string parameterName) : base(parameters)
        {
            Operand = operand;
            Filter = filter;
            ParameterName = parameterName;
        }

        public LambdaMethodOperator(IDictionary<string, ParameterExpression> parameters, FilterPart operand) : base(parameters)
        {
            Operand = operand;
        }

        public FilterPart Operand { get; }
        public FilterPart Filter { get; }
        public string ParameterName { get; }

        public override Expression Build() => Build(Operand.Build());

        private Expression Build(Expression operandExpression)
        {
            Func<Expression, Expression[], MethodCallExpression> anyMethodCall = GetMethod(operandExpression.Type);

            if (Filter == null)
                return anyMethodCall(operandExpression, new Expression[] { });

            if (!this.Parameters.ContainsKey(ParameterName))
            {
                this.Parameters.Add
                (
                    ParameterName, 
                    Expression.Parameter(operandExpression.Type.GetUnderlyingElementType(), ParameterName)
                );
            }

            var expression = anyMethodCall
            (
                operandExpression,
                new Expression[]
                {
                    Expression.Lambda
                    (
                        typeof(Func<,>).MakeGenericType
                        (
                            new Type[]
                            {
                                this.Parameters[ParameterName].Type,
                                typeof(bool)
                            }
                        ),
                        ConvertBody(Filter.Build()),
                        this.Parameters[ParameterName]
                    )
                }
            );

            this.Parameters.Remove(ParameterName);

            return expression;
        }

        protected abstract Func<Expression, Expression[], MethodCallExpression> GetMethod(Type operandExpressionType);

        protected static Expression ConvertBody(Expression body)
            => body.Type != typeof(bool)
                ? Expression.Convert(body, typeof(bool))
                : body;
    }
}
