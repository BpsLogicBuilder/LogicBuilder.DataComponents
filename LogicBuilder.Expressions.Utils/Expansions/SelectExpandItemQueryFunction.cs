using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using LogicBuilder.Expressions.Utils.Strutures;
using System;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    public class SelectExpandItemQueryFunction
    {
        [Obsolete("Use SortCollection.", false)]
        public IExpressionDescriptor MethodCallDescriptor { get; set; }
        public SortCollection SortCollection { get; set; }
    }
}
