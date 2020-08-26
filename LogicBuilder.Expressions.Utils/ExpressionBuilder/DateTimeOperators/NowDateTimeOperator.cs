using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators
{
    public class NowDateTimeOperator : IExpressionPart
    {
        public NowDateTimeOperator()
        {
        }

        public Expression Build() => LinqHelpers.GetNowDateTimOffsetProperty();
    }
}
