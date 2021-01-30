using LogicBuilder.Expressions.Utils.ExpressionBuilder.Lambda;
using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using System;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    public class ExpansionFilterOption
    {
        [Obsolete("Use FilterLambdaOperator.", false)]
        public FilterLambdaDescriptor Filter { get; set; }
        public FilterLambdaOperator FilterLambdaOperator { get; set; }
    }
}
