using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.DateTimeOperators
{
    public class MinDateTimeOperator : IExpressionPart
    {
        public MinDateTimeOperator()
        {
        }

        public Expression Build() => LinqHelpers.GetMinDateTimOffsetField();
    }
}
