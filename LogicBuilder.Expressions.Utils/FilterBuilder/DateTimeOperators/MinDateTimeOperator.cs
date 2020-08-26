using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.FilterBuilder.DateTimeOperators
{
    public class MinDateTimeOperator : IExpressionPart
    {
        public MinDateTimeOperator()
        {
        }

        public Expression Build() => LinqHelpers.GetMinDateTimOffsetField();
    }
}
