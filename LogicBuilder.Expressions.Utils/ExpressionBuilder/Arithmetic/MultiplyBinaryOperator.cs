namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class MultiplyBinaryOperator : BinaryOperator
    {
        public MultiplyBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.mul;
    }
}
