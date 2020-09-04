using System.Reflection;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class NotEqualsBinaryOperatorHandler : EqualityBinaryOperatorHandlerBase
    {
        public NotEqualsBinaryOperatorHandler(IExpressionPart left, IExpressionPart right, FilterFunction @operator) : base(left, right, @operator)
        {
        }

        protected override MethodInfo CompareMethodInfo => LinqHelpers.ByteArraysNotEqualMethodInfo;
    }
}
