using System.Collections.Generic;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.Strutures
{
    public class FilteredIncludeExpression
    {
        public LambdaExpression Include { get; set; }
        public LambdaExpression Filter { get; set; }
        public ICollection<FilteredIncludeExpression> FilteredIncludes { get; set; }
    }
}
