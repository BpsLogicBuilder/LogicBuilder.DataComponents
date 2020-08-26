using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ParameterOperator : FilterPart
    {
        public ParameterOperator(IDictionary<string, ParameterExpression> parameters, string parameterName)
        {
            ParameterName = parameterName;
            Parameters = parameters;
        }

        public IDictionary<string, ParameterExpression> Parameters { get; }
        public string ParameterName { get; }

        public override Expression Build() => Parameters[ParameterName];
    }
}
