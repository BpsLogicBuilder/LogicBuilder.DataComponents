namespace LogicBuilder.Expressions.Utils.ExpressionBuilder.Arithmetic
{
    public class SubtractBinaryOperator : BinaryOperator
    {
        public SubtractBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.sub;
    }
}
