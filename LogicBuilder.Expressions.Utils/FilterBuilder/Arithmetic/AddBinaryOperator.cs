namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class AddBinaryOperator : BinaryOperator
    {
        public AddBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.add;
    }
}