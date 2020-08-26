namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class SubtractBinaryOperator : BinaryOperator
    {
        public SubtractBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.sub;
    }
}
