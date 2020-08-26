using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class NowDateTimeOperator : IExpressionPart
    {
        public NowDateTimeOperator()
        {
        }

        public Expression Build() => LinqHelpers.GetNowDateTimOffsetProperty();
    }
}
