using LogicBuilder.Expressions.Utils.ExpressionDescriptors;
using LogicBuilder.Expressions.Utils.Strutures;
using System;

namespace LogicBuilder.Expressions.Utils.Expansions
{
    public class ExpansionQueryOption
    {
        [Obsolete("Use SortCollection.", false)]
        public IExpressionDescriptor QueryFunction { get; set; }
        public SortCollection SortCollection { get; set; }
    }
}
