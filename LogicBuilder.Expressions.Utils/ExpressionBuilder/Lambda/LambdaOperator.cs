using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class LambdaOperator : IExpressionPart
    {
        public LambdaOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart selector, Type sourceElementType, string parameterName)
        {
            Selector = selector;
            SourceElementType = sourceElementType;
            ParameterName = parameterName;
            Parameters = parameters;
        }

        public IExpressionPart Selector { get; }
        public Type SourceElementType { get; }
        public string ParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build()
        {
            if (!this.Parameters.ContainsKey(ParameterName))
            {
                this.Parameters.Add
                (
                    ParameterName,
                    Expression.Parameter(SourceElementType, ParameterName)
                );
            }

            var selectorBody = Selector.Build();
            var expression = Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType
                (
                    new Type[]
                    {
                        this.Parameters[ParameterName].Type,
                        selectorBody.Type
                    }
                ),
                selectorBody,
                this.Parameters[ParameterName]
            );

            this.Parameters.Remove(ParameterName);

            return expression;
        }
    }
}
