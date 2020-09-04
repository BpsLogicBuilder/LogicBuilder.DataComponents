using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class SelectorLambdaOperator : IExpressionPart
    {
        public SelectorLambdaOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart selector, Type sourceElementType, string parameterName)
        {
            Selector = selector;
            SourceElementType = sourceElementType;
            ParameterName = parameterName;
            Parameters = parameters;
        }

        public SelectorLambdaOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart selector, Type sourceElementType, Type bodyType, string parameterName)
        {
            Selector = selector;
            SourceElementType = sourceElementType;
            ParameterName = parameterName;
            Parameters = parameters;
            BodyType = bodyType;
        }

        public IExpressionPart Selector { get; }
        public Type SourceElementType { get; }
        public Type BodyType { get; private set; }
        public string ParameterName { get; }
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public Expression Build()
        {
            this.Parameters.Add
            (
                ParameterName,
                Expression.Parameter(SourceElementType, ParameterName)
            );

            var selectorBody = Selector.Build();
            if (BodyType == null)
                BodyType = selectorBody.Type;

            var expression = Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType
                (
                    new Type[]
                    {
                        this.Parameters[ParameterName].Type,
                        BodyType
                    }
                ),
                ConvertBody(selectorBody),
                this.Parameters[ParameterName]
            );

            this.Parameters.Remove(ParameterName);

            return expression;
        }

        private Expression ConvertBody(Expression body)
            => body.Type != BodyType
                ? Expression.Convert(body, BodyType)
                : body;
    }
}
