using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    public class ParameterOperator : FilterPart
    {
        public ParameterOperator(IDictionary<string, ParameterExpression> parameters, string parameterName) : base(parameters)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; set; }

        public override Expression Build() => Parameters[ParameterName];
    }
}
