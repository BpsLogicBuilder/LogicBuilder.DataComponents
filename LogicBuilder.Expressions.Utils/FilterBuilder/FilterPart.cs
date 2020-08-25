using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder
{
    abstract public class FilterPart
    {
        abstract public Expression Build();
        public IDictionary<string, ParameterExpression> Parameters { get; }

        public FilterPart(IDictionary<string, ParameterExpression> parameters)
        {
            this.Parameters = parameters;
        }
    }
}
