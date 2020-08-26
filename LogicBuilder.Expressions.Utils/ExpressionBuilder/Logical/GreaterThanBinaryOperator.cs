namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Logical
{
    public class GreaterThanBinaryOperator : BinaryOperator
    {
        public GreaterThanBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.gt;
    }
}
