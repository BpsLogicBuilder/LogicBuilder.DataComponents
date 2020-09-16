using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class FilterLambdaOperator : IExpressionPart
    {
        public FilterLambdaOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart filterBody, Type sourceElementType, string parameterName)
        {
            FilterBody = filterBody;
            SourceElementType = sourceElementType;
            ParameterName = parameterName;
            Parameters = parameters;
        }

        public IExpressionPart FilterBody { get; }
        public Type SourceElementType { get; }
        public string ParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build()
        {
            this.Parameters.Add
            (
                ParameterName,
                Expression.Parameter(SourceElementType, ParameterName)
            );

            var expression = Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType
                (
                    new Type[]
                    {
                        this.Parameters[ParameterName].Type,
                        typeof(bool)
                    }
                ),
                ConvertBody(FilterBody.Build()),
                this.Parameters[ParameterName]
            );

            this.Parameters.Remove(ParameterName);

            return expression;
        }

        private Expression ConvertBody(Expression body)
            => body.Type != typeof(bool)
                ? Expression.Convert(body, typeof(bool))
                : body;
    }
}
