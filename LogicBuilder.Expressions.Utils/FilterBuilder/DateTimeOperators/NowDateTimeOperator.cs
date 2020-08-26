using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class NowDateTimeOperator : FilterPart
    {
        public NowDateTimeOperator()
        {
        }

        public override Expression Build() => LinqHelpers.GetNowDateTimOffsetProperty();
    }
}
