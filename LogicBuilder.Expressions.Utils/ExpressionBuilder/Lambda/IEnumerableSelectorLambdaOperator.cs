using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda
{
    public class IEnumerableSelectorLambdaOperator : IExpressionPart
    {
        public IEnumerableSelectorLambdaOperator(IDictionary<string, ParameterExpression> parameters, IExpressionPart selector, Type sourceElementType, string parameterName)
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
            this.Parameters.Add
            (
                ParameterName,
                Expression.Parameter(SourceElementType, ParameterName)
            );

            var selectorBody = Selector.Build();

            var expression = Expression.Lambda
            (
                typeof(Func<,>).MakeGenericType
                (
                    new Type[]
                    {
                        this.Parameters[ParameterName].Type,
                        typeof(IEnumerable<>).MakeGenericType//specifically using IEnumerable<T> (vs ICollection<T> etc) for the Func return type
                        (
                            GetUnderlyingType(selectorBody)
                        )
                    }
                ),
                selectorBody,//don't have to convert the body. The type can remain ICollection<T>
                this.Parameters[ParameterName]
            );

            this.Parameters.Remove(ParameterName);

            return expression;
        }

        private Type GetUnderlyingType(Expression expression)
        {
            if (expression.Type.IsGenericType && expression.Type.GetGenericTypeDefinition() == typeof(System.Linq.IGrouping<,>))
                return expression.Type.GetGenericArguments()[1];

            return expression.GetUnderlyingElementType();
        }
    }
}
