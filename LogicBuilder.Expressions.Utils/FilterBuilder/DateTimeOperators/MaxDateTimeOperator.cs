using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class MaxDateTimeOperator : FilterPart
    {
        public MaxDateTimeOperator()
        {
        }

        public override Expression Build() => LinqHelpers.GetMaxDateTimOffsetField();
    }
}
