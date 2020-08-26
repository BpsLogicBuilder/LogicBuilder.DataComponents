using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class MaxDateTimeOperator : IExpressionPart
    {
        public MaxDateTimeOperator()
        {
        }

        public Expression Build() => LinqHelpers.GetMaxDateTimOffsetField();
    }
}
