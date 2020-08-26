namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class GreaterThanBinaryOperator : BinaryOperator
    {
        public GreaterThanBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.gt;
    }
}
