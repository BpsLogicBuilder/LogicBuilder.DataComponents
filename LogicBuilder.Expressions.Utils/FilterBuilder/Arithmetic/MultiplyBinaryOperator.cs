namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class MultiplyBinaryOperator : BinaryOperator
    {
        public MultiplyBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.mul;
    }
}
