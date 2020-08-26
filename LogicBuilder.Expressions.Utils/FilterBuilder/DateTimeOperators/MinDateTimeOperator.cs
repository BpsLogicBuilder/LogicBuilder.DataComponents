using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class MinDateTimeOperator : FilterPart
    {
        public MinDateTimeOperator()
        {
        }

        public override Expression Build() => LinqHelpers.GetMinDateTimOffsetField();
    }
}
