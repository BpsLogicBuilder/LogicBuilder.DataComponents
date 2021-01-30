using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using System;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    public class SelectExpandItemFilter
    {
        [Obsolete("Use FilterLambdaOperator.", false)]
        public IExpressionDescriptor FilterBody { get; set; }
        
        [Obsolete("Use FilterLambdaOperator.", false)]
        public string ParameterName { get; set; }

        public FilterLambdaOperator FilterLambdaOperator { get; set; }
    }
}
