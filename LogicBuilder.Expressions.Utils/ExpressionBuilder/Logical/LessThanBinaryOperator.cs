using System;
using System.Linq.Expressions;

namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class LessThanBinaryOperator : BinaryOperator
    {
        public LessThanBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
            BinaryOperatorHandler = new GreaterThanLessThanBinaryOperatorHandler(Left, Right, Operator);
        }

        protected override BinaryOperatorHandler BinaryOperatorHandler { get; }

        public override FilterFunction Operator => FilterFunction.lt;
    }
}
