using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.Lambda
{
    public class FilterLambdaOperator : FilterPart
    {
        public FilterLambdaOperator(IDictionary<string, ParameterExpression> parameters, FilterPart selector, Type sourceElementType, string parameterName) : base(parameters)
        {
            Selector = selector;
            SourceElementType = sourceElementType;
            ParameterName = parameterName;
        }

        public FilterPart Selector { get; }
        public Type SourceElementType { get; }
        public string ParameterName { get; }

        public override Expression Build() => Build1();

        private Expression Build1()
        {
            if (!this.Parameters.ContainsKey(ParameterName))
            {
                this.Parameters.Add
                (
                    ParameterName,
                    Expression.Parameter(SourceElementType, ParameterName)
                );
            }

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
                ConvertBody(Selector.Build()),
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
