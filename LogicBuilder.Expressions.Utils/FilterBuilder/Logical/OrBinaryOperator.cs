namespace LogicBuilder.Expressions.Utils.FilterBuilder.Logical
{
    public class OrBinaryOperator : BinaryOperator
    {
        public OrBinaryOperator(FilterPart left, FilterPart right) : base(left, right)
        {
        }

        public override FilterFunction Operator => FilterFunction.or;
    }
}
