using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ParameterOperator : IExpressionPart
    {
        public ParameterOperator(IDictionary<string, ParameterExpression> parameters, string parameterName)
        {
            ParameterName = parameterName;
            Parameters = parameters;
        }

        public IDictionary<string, ParameterExpression> Parameters { get; }
        public string ParameterName { get; }

        public Expression Build() => Parameters[ParameterName];
    }
}
