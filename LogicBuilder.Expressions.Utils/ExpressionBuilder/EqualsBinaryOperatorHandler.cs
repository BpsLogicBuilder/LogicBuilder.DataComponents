using System.Linq.Expressions;
using System.Reflection;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder
{
    public class EqualsBinaryOperatorHandler : EqualityBinaryOperatorHandlerBase
    {
        public EqualsBinaryOperatorHandler(IExpressionPart left, IExpressionPart right, FilterFunction @operator) : base(left, right, @operator)
        {
        }

        protected override MethodInfo CompareMethodInfo => LinqHelpers.ByteArraysEqualMethodInfo;
    }
}
