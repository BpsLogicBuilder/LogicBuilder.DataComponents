namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class OrBinaryOperator : BinaryOperator
    {
        public OrBinaryOperator(IExpressionPart left, IExpressionPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.or;
    }
}
