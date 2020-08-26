namespace LogicBuilder.Expressions.Utils.FilterBuilder.Arithmetic
{
    public class SubtractBinaryOperator : BinaryOperator
    {
        public SubtractBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.sub;
    }
}
